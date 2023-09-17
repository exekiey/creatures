using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

public struct Cell
{
    public int x;
    public int y;


    public Cell(int x, int y)
    {

        this.x = x;
        this.y = y;

    }

    public override bool Equals(object obj)
    {
        if (obj is Cell otherCell)
        {
            return x == otherCell.x && y == otherCell.y;
        }
        return false;
    }

    public static bool operator ==(Cell left, Cell right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(Cell left, Cell right)
    {
        return !left.Equals(right);
    }

    public string ToString()
    {

        return $"Cell ({x}, {y})";
    }

}


public class GridScript : MonoBehaviour
{

    [SerializeField] float gridSize;

    [SerializeField] bool drawGrid;

    static GridScript instance;

    public static float GridSize { get => instance.gridSize; }
    public float GridSize1 { get => gridSize; set => gridSize = value; }

    [SerializeField] GameObject test;

    public void Awake()
    {

        instance = this;

        PathFinding.SetObstacleNodes();

    }

    public static Cell GetCellCoords(Vector2 position)
    {

        return new Cell { x = (int)Mathf.Floor(position.x / instance.gridSize), y = (int)Mathf.Floor(position.y / instance.gridSize) };

    }

    public static Vector2 GetRealWorldCoords(Cell position)
    {

        return new Vector2((position.x * instance.gridSize) + instance.gridSize / 2, (position.y * instance.gridSize) + instance.gridSize / 2);

    }

    public static Vector2 GetRealWorldCoords(Vector2 position)
    {

        return new Vector2((position.x * instance.gridSize) + instance.gridSize / 2, (position.y * instance.gridSize) + instance.gridSize / 2);

    }

    public static bool IsCellOccupied(Vector2 cell)
    {

        Vector2 cellCentre = GetRealWorldCoords(cell);
        Collider2D collider2D = Physics2D.OverlapPoint(cellCentre);

        return collider2D != null;

    } 
    
    public static bool IsCellOccupied(Cell cell)
    {

        Vector2 cellCentre = GetRealWorldCoords(cell);
        Collider2D collider2D = Physics2D.OverlapPoint(cellCentre, LayerMask.GetMask("obstacle"));

        return collider2D != null;

    }

    public static Vector2 GetSizeInCells(Vector2 size)
    {

        /*
        float error = 0.01f;

        size -= Vector2.one * error;
        */
        return new Vector2(Mathf.Ceil(size.x / GridSize), Mathf.Ceil(size.y / GridSize));

    }
    
    public static Vector2 RoundToCell(Vector2 position)
    {

        Cell cell = GetCellCoords(position);
        Vector2 pos = GetRealWorldCoords(cell);

        return pos;

        return new Vector2(Mathf.Floor(position.x / instance.gridSize), Mathf.Floor(position.y / instance.gridSize));
    }

    private void OnDrawGizmos()
    {
        float cameraHeight = Camera.main.orthographicSize;

        float cameraWidth = cameraHeight * Camera.main.aspect;

        if (drawGrid)
        {


        if (gridSize > 0)
        {

            for (float i = 0 ; i > -cameraHeight; i -= gridSize)
            {

                Gizmos.DrawLine(new Vector2(-cameraWidth, i), new Vector3(cameraWidth, i));

            }

            for (float i = 0; i < cameraHeight; i += gridSize)
            {

                Gizmos.DrawLine(new Vector2(-cameraWidth, i), new Vector3(cameraWidth, i));

            }

            for (float i = 0; i > -cameraWidth; i -= gridSize)
            {

                Gizmos.DrawLine(new Vector2(i, -cameraHeight), new Vector3(i, cameraHeight));

            }

            for (float i = 0; i < cameraWidth; i += gridSize)
            {

                Gizmos.DrawLine(new Vector2(i, -cameraHeight), new Vector3(i, cameraHeight));

            }


            }
        }
        /*
        Gizmos.color = new Color(1, 1, 1, 0.1f);
        Gizmos.DrawLine(new Vector2(0, -cameraHeight), new Vector2(0, cameraHeight));
        Gizmos.DrawLine(new Vector2(-cameraWidth, 0), new Vector2(cameraWidth, 0));*/
    }

}
