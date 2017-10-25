using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour {

    public float sightRadius = 5f;
    public LayerMask layerMask;
    public LayerMask layerMask2;
    public Material defaultMaterial;
    public Material customMaterial;
    private MeshFilter meshFilter;
    private LevelManager levelManager;

    public Vector2 boxSize;

    public struct VertexPosWithAngle {
        public Vector2 vertexPos;
        public float angle;

        public VertexPosWithAngle(Vector3 vertexPos, float angle) {
            this.vertexPos = vertexPos;
            this.angle = angle;
        }
    }

    public void Awake() {
        levelManager = GameObject.FindObjectOfType<LevelManager>();
    }

    void Update() {
//        print(Time.deltaTime);
//        levelManager.blockers.ForEach(blocker => blocker.setMaterial(customMaterial));

        meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        List<VertexPosWithAngle> viewPoints = new List<VertexPosWithAngle>();
        Collider2D[] blockersInRange = Physics2D.OverlapCircleAll(transform.position, sightRadius, layerMask2);
        foreach (Collider2D col in blockersInRange) {
            if (!(col is PolygonCollider2D))
                continue;
            PolygonCollider2D polyCol = (PolygonCollider2D)col;
            Vector2 polyPos = polyCol.transform.position;
            foreach (Vector2 point in polyCol.points) {
                Vector2 viewPointVertexPos = polyPos + point;
//                Debug.DrawLine(new Vector2(vertexPos.x, vertexPos.y - 0.05f), new Vector2(vertexPos.x, vertexPos.y + 0.05f));
                viewPoints.Add(new VertexPosWithAngle(viewPointVertexPos, Vector2.SignedAngle(Vector2.one, viewPointVertexPos - (Vector2)transform.position)));
            }
        }

        viewPoints.Sort((v1, v2) => v1.angle.CompareTo(v2.angle));
        List<Vector2> hitPoints = new List<Vector2>();
        foreach (VertexPosWithAngle vertexPos in viewPoints) {
//        VertexPosWithAngle vertexPos = viewPoints[0];
            Debug.DrawLine(new Vector2(vertexPos.vertexPos.x, vertexPos.vertexPos.y - 0.05f), new Vector2(vertexPos.vertexPos.x, vertexPos.vertexPos.y + 0.05f));
            Vector2 direction = vertexPos.vertexPos - (Vector2)transform.position;
//            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.001f, direction, sightRadius, layerMask);
//            RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxSize, 0, direction, layerMask);
            Vector2 endPos = (Vector2)transform.position + direction * sightRadius;
            RaycastHit2D hit = Physics2D.Linecast(transform.position, endPos, layerMask);
            if (hit.collider != null) {
                hitPoints.Add(hit.point);
//            hit.collider.gameObject.GetComponent<SpriteRenderer>().material = defaultMaterial;

                Vector2 newDirection = (Quaternion.AngleAxis(0.1f, Vector3.forward) * direction).normalized;
//                RaycastHit2D newHit = Physics2D.CircleCast(transform.position, 0.001f, newDirection, sightRadius, layerMask);
                RaycastHit2D newHit = Physics2D.Linecast(transform.position, (Vector2)transform.position + newDirection * sightRadius, layerMask);
                if (newHit.collider != null) {
                    hitPoints.Add(newHit.point);
                }
                else {
                    hitPoints.Add((Vector2)transform.position + newDirection * sightRadius);
                }
                newDirection = (Quaternion.AngleAxis(-0.1f, Vector3.forward) * direction).normalized;
//                newHit =  Physics2D.CircleCast(transform.position, 0.001f, newDirection, sightRadius, layerMask);
                newHit = Physics2D.Linecast(transform.position, (Vector2)transform.position + newDirection * sightRadius, layerMask);
                if (newHit.collider != null) {
                    hitPoints.Add(newHit.point);
                }
                else {
                    hitPoints.Add((Vector2)transform.position + newDirection * sightRadius);
                }

            }
            else {
                hitPoints.Add((Vector2)transform.position + direction * sightRadius);
            }
        }

        for (int i = 0; i < hitPoints.Count; i++)
            Debug.DrawLine(transform.position, hitPoints[i]);

        if (hitPoints.Count == 0)
            return;

        hitPoints.Add(hitPoints[0]);
        int vertexCount = hitPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector2.zero;
        for (int i = 0; i < vertexCount - 1; i++) {
            vertices[i + 1] = transform.InverseTransformPoint(hitPoints[i]);
            if (i < vertexCount - 2) {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 2;
                triangles[i * 3 + 2] = i + 1;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
