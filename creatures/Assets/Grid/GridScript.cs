using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

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

}


public class GridScript : MonoBehaviour
{

    [SerializeField] float gridSize;

    [SerializeField] bool drawGrid;

    static GridScript instance;

    public static float GridSize { get => instance.gridSize; }

    public void Awake()
    {
        instance = this;

        GameObject gameObject = new GameObject();

    }

    private void Start()
    {
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
        Collider2D collider2D = Physics2D.OverlapPoint(cellCentre);

        return collider2D != null;

    }
    
    private void OnDrawGizmos()
    {
        float cameraHeight = Camera.main.orthographicSize;

        float cameraWidth = cameraHeight * Camera.main.aspect;
        
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
        /*
        Gizmos.color = new Color(1, 1, 1, 0.1f);
        Gizmos.DrawLine(new Vector2(0, -cameraHeight), new Vector2(0, cameraHeight));
        Gizmos.DrawLine(new Vector2(-cameraWidth, 0), new Vector2(cameraWidth, 0));*/
    }

}
