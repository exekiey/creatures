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
    }

    public static Cell GetCellCoords(Vector2 position)
    {

        return new Cell { x = (int)Mathf.Floor(position.x / instance.gridSize), y = (int)Mathf.Floor(position.y / instance.gridSize) };

    }

    public static Vector2 GetRealWorldCoords(Cell position)
    {

        return new Vector2((position.x * instance.gridSize) + instance.gridSize / 2, (position.y * instance.gridSize) + instance.gridSize / 2);

    }
    
    public static bool IsCellOccupied(Cell cell)
    {

        Vector2 cellCentre = GetRealWorldCoords(cell);
        Collider2D collider2D = Physics2D.OverlapPoint(cellCentre);

        return collider2D != null;

    }


    public static List<Cell> FilterNeighbors(params Cell[] args)
    {

        List<Cell> neighbors = new List<Cell>();

        foreach (Cell cell in args)
        {

            if (!IsCellOccupied(cell))
            {
                neighbors.Add(cell);
            }

        }

        return neighbors;

    }

    public static List<Cell> GetNeighborCells(Cell cell)
    {



        Cell up = new Cell { x = cell.x, y = cell.y + 1} ;

        Cell upLeft = new Cell { x = cell.x - 1, y = cell.y + 1 };

        Cell upRight = new Cell { x = cell.x + 1, y = cell.y + 1 };

        Cell down = new Cell { x = cell.x, y = cell.y - 1} ;
        Cell downLeft = new Cell { x = cell.x - 1, y = cell.y - 1 };
        Cell downRight = new Cell { x = cell.x + 1, y = cell.y - 1 };

        Cell left = new Cell { x = cell.x - 1, y = cell.y};
        Cell right = new Cell { x = cell.x + 1, y = cell.y};


        List<Cell> neighbors = FilterNeighbors(up, upLeft, upRight, down, downLeft, downRight, left, right);

        return neighbors;

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
