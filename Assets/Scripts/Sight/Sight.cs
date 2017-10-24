using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour {

    private MeshFilter meshFilter;

    void Awake() {
        meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;
        mesh.Clear();

//        List<Vector2> viewPoints = new List<Vector2>();
//        viewPoints.Add(new Vector2(-2, 2));
//        viewPoints.Add(new Vector2(2, 2));
        int viewPoints = 361;
        int vertexCount = viewPoints + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];
        vertices[0] = Vector3.zero;

        for (int i = 0; i < vertexCount - 1; i++) {
            Vector3 pos = Quaternion.AngleAxis(i, Vector3.back) * Vector3.up;
            vertices[i + 1] = pos * 5;
            if (i < vertexCount - 2) {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    void Update() {
        
    }
}
