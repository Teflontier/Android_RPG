using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour {

    public float sightRadius = 10f;
    public LayerMask layerMask;
    private MeshFilter meshFilter;

    public struct VertexPosWithAngle {
        public Vector2 vertexPos;
        public float angle;

        public VertexPosWithAngle(Vector3 vertexPos, float angle) {
            this.vertexPos = vertexPos;
            this.angle = angle;
        }
    }

    void Update() {
        meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        List<VertexPosWithAngle> viewPoints = new List<VertexPosWithAngle>();
        Collider2D[] blockersInRange = Physics2D.OverlapCircleAll(transform.position, sightRadius, layerMask);
        foreach (Collider2D col in blockersInRange) {
            PolygonCollider2D polyCol = (PolygonCollider2D)col;
            Vector2 polyPos = polyCol.transform.position;
            foreach (Vector2 point in polyCol.points) {
                Vector2 vertexPos = polyPos + point;
                viewPoints.Add(new VertexPosWithAngle(vertexPos, Vector2.Angle(transform.position, vertexPos)));
            }
        }

        viewPoints.Sort((v1, v2) => v1.angle.CompareTo(v2.angle));
        List<Vector2> hitPoints = new List<Vector2>();
        foreach (VertexPosWithAngle vertexPos in viewPoints) {
            RaycastHit2D hit = Physics2D.Linecast(transform.position, vertexPos.vertexPos, layerMask);
            if (hit.collider != null)
                hitPoints.Add(hit.point);
            else{
                Vector2 direction = (vertexPos.vertexPos - (Vector2)transform.position).normalized;
                hitPoints.Add((Vector2)transform.position + direction * sightRadius);
            }
        }

        for (int i = 0; i < hitPoints.Count; i++)
            Debug.DrawLine(transform.position, hitPoints[i]);

        if (hitPoints.Count == 0)
            return;

        int vertexCount = hitPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector2.zero;
        for (int i = 0; i < vertexCount - 1; i++) {
            vertices[i + 1] = transform.InverseTransformPoint(hitPoints[i]);
            if (i < vertexCount - 2) {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
