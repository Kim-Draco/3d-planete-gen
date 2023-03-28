using System.Collections;
using System.Collections.Generic;
// Builds a Mesh containing a single triangle with uvs.
// Create arrays of vertices, uvs and triangles, and copy them into the mesh.

using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class triangle : MonoBehaviour
{
    // Use this for initialization
    Mesh mesh;
    Mesh mesh2;
    Mesh mesh3;
    Vector3[] vertices, vertices2;
    int[] triangles, triangles2;

    void Awake()
    {
            //mesh = GetComponent<MeshFilter>().mesh;
            //mesh2 = GetComponent<MeshFilter>().mesh;
    }
    void Start()
    {
        
        MakeMeshData(0);
        CreateMesh(0);
        
        //gameObject.AddComponent<MeshFilter>();
        //gameObject.AddComponent<MeshRenderer>();
        

        //mesh.Clear();

        // make changes to the Mesh by creating arrays which contain the new values
        //mesh.vertices = new Vector3[] {new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0)};
        //mesh.uv = new Vector2[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1)};
        //mesh.triangle =  new int[] {0, 1, 2};
    }

    void MakeMeshData(int i)
    {
        vertices = new Vector3[] {new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0)};
        triangles =  new int[] {0, 1, 2};
        triangles2 =  new int[] {0, 2, 1};
    }

    void CreateMesh(int i)
    {
        //mesh[i].Clear();
        CombineInstance[] combine = new CombineInstance[2];
        //mesh.vertices = vertices;
        //mesh.triangles = triangles;
        //mesh2.vertices = vertices;
        //mesh2.triangles = triangles2;
        //mesh.RecalculateNormals();
        //mesh2.RecalculateNormals();
        combine[0].mesh = GetComponent<MeshFilter>().mesh;
        combine[0].mesh.vertices = vertices;
        combine[0].mesh.triangles = triangles;
        combine[0].mesh.RecalculateNormals();
        combine[1].mesh = GetComponent<MeshFilter>().mesh;
        combine[1].mesh.vertices = vertices;
        combine[1].mesh.triangles = triangles2;
        combine[1].mesh.RecalculateNormals();
        mesh3.CombineMeshes(combine);
    }
}
