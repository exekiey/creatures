using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CustomLineRenderer : MonoBehaviour
{

    Vector2[] points;

    Vector3[] vertices;
    int[] triangles;

    Mesh mesh;

    [SerializeField] float width = 0.2f;

    int counter = 0;
    int verticesCounter = 0;
    int trianglesCounter = 0;

    public int NumberOfPoints
    {
        set
        {
            int numberOfPoints = value;


            points = new Vector2[numberOfPoints];
            vertices = new Vector3[numberOfPoints * 2];
            triangles = new int[numberOfPoints * 6];



        }


    }

    public void SetPoints(Vector2[] positions)
    {
        int numberOfPoints = positions.Length;

        points[counter] = positions[0];

        Vector2 direction = (positions[1] - positions[0]).normalized;
        Vector2 perpendicular = Vector2.Perpendicular(direction);



        vertices[verticesCounter] = points[counter] + perpendicular * width / 2;
        verticesCounter++;

        vertices[verticesCounter] = points[counter] - perpendicular * width / 2;
        verticesCounter++;

        counter++;

        for (int i = 1; i < numberOfPoints; i++)
        {
            LocateNextSegment(positions[i]);
        }

        counter = 0;
        verticesCounter = 0;
        trianglesCounter = 0;

    }
    private void LocateNextSegment(Vector2 pos)
    {
        points[counter] = pos;

        Vector2 direction = points[counter - 1] - points[counter];
        Vector2 perpendicular = Vector2.Perpendicular(direction).normalized;

        vertices[verticesCounter] = points[counter] - perpendicular * width / 2;
        verticesCounter++;

        vertices[verticesCounter] = points[counter] + perpendicular * width / 2;
        verticesCounter++;

        triangles[trianglesCounter] = verticesCounter - 1;
        trianglesCounter++;
        triangles[trianglesCounter] = verticesCounter - 3;
        trianglesCounter++;
        triangles[trianglesCounter] = verticesCounter - 2;
        trianglesCounter++;

        triangles[trianglesCounter] = verticesCounter - 2;
        trianglesCounter++;
        triangles[trianglesCounter] = verticesCounter - 3;
        trianglesCounter++;
        triangles[trianglesCounter] = verticesCounter - 4;
        trianglesCounter++;

        counter++;
    }

    public void Start()
    {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        UpdateMesh();
    }



    private void Update()
    {
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    private void OnDrawGizmos()
    {


        if (points != null)
        {

            foreach (Vector2 point in points)
            {

                Gizmos.DrawIcon(point, "sv_icon_dot6_pix16_gizmo", true);

            }

        }
    }



}
