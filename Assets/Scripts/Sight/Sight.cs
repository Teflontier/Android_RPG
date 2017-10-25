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
    private Mesh mesh;
    private LevelManager levelManager;
    private List<Vector2> viewPoints = new List<Vector2>();
    private List<VertexPosWithAngle> hitPoints = new List<VertexPosWithAngle>();

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
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;
    }

    void Update() {
        levelManager.blockers.ForEach(blocker => blocker.setMaterial(customMaterial));
        viewPoints.Clear();
        hitPoints.Clear();

        Collider2D[] blockersInRange = Physics2D.OverlapCircleAll(transform.position, sightRadius, layerMask2);
        foreach (Collider2D col in blockersInRange) {
            if (!(col is PolygonCollider2D))
                continue;
            PolygonCollider2D polyCol = (PolygonCollider2D)col;
            Vector2 polyPos = polyCol.transform.position;
            foreach (Vector2 point in polyCol.points) {
                Vector2 viewPointVertexPos = polyPos + point;
                viewPoints.Add(viewPointVertexPos);
            }
        }

        foreach (Vector2 vertexPos in viewPoints) {
            Vector2 direction = vertexPos - (Vector2)transform.position;
            RaycastHit2D hit = getLineCastHit((vertexPos - (Vector2)transform.position).normalized);
            if (hit.collider != null) {
                Vector2 normal = new Vector2(direction.y, -direction.x);
                getLineCastHit(((hit.point + normal * 0.1f) - (Vector2)transform.position).normalized);
                getLineCastHit(((hit.point - normal * 0.1f) - (Vector2)transform.position).normalized);
            }
        }

        for (int i = 0; i < 30; i++)
            getLineCastHit((Quaternion.AngleAxis(i * 12, Vector3.forward) * Vector2.right).normalized);
            

        for (int i = 0; i < hitPoints.Count; i++)
            Debug.DrawLine(transform.position, hitPoints[i].vertexPos);

        if (hitPoints.Count == 0)
            return;

        hitPoints.Sort((h1, h2) => h1.angle.CompareTo(h2.angle));
        hitPoints.Add(hitPoints[0]);
        int vertexCount = hitPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector2.zero;
        for (int i = 0; i < vertexCount - 1; i++) {
            vertices[i + 1] = transform.InverseTransformPoint(hitPoints[i].vertexPos);
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

    private RaycastHit2D getLineCastHit(Vector2 direction) {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, (Vector2)transform.position + direction * sightRadius, layerMask);
        if (hit.collider != null) {
            hitPoints.Add(new VertexPosWithAngle(hit.point, Vector2.SignedAngle(Vector2.one, hit.point - (Vector2)transform.position)));
            hit.collider.gameObject.GetComponentInParent<SpriteRenderer>().material = defaultMaterial;
        }
        else {
            Vector2 pos = (Vector2)transform.position + direction * sightRadius;
            hitPoints.Add(new VertexPosWithAngle(pos, Vector2.SignedAngle(Vector2.one, pos - (Vector2)transform.position)));
        }
        return hit;
    }
}
