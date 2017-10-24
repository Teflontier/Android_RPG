using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour {

    private MeshFilter meshFilter;

    void Awake(){
        meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;
        mesh.Clear();
        mesh.vertices = new Vector3[]{new Vector3(-10,0,0), new Vector3(-10,10,0),new Vector3(10,10,0),new Vector3(10,-10,0)};
        mesh.triangles = new int[]{0,1,2, 2,3,0};
        mesh.RecalculateNormals();
    }

    void Update(){
        
    }
}
