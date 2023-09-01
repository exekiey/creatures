using UnityEngine;

public class MeshTest : MonoBehaviour
{
    void Start()
    {

        Debug.Log("Test");

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4];
        Vector2[] uv = new Vector2[4];
        int[] triangles = new int[6];


        vertices[0] = new Vector3(0, 0);
        vertices[1] = new Vector3(0, 1);
        vertices[2] = new Vector3(1, 1);
        vertices[3] = new Vector3(1, 0);
        
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;
        
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        GetComponent<MeshFilter>().mesh = mesh;
        
    }
}
