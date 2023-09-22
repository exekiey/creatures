using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class Node
{

    int _x;
    int _y;

    float _distanceFromOrigin;
    float _distanceToDestination;

    Node _parent;

    static (int, int) destination;

    public static (int, int) Destination { set => destination = value; }

    public Node(int x, int y, Node parent, float distanceFromParent)
    {

        this._x= x;
        this._y = y;

        _parent = parent;

        _distanceFromOrigin = distanceFromParent + parent._distanceFromOrigin;

        _distanceToDestination = Mathf.Sqrt(Mathf.Pow(this._x - destination.Item1, 2) + Mathf.Pow(this._y - destination.Item2, 2));

    }
    public Node(int x, int y)
    {

        this._x = x;
        this._y = y;

        _distanceFromOrigin = 0;

        _distanceToDestination = Mathf.Sqrt(Mathf.Pow(this._x - destination.Item1, 2) + Mathf.Pow(this._y - destination.Item2, 2));

    }

    public override string ToString()
    {
        return $"Node ({_x}, {_y})";
    }

    public LinkedList<Node> Path
    {

        get
        {

            LinkedList<Node> path = new LinkedList<Node>();

            Node currentNode = this;

            while (currentNode._distanceFromOrigin != 0)
            {

                path.AddFirst(currentNode);
                currentNode = currentNode._parent;

                //Debug.Log(currentNode.X + ":" + currentNode.Y); 

            }

            path.AddFirst(currentNode);

            return path;

        }


    }




    public static bool operator <(Node left, Node right)
    {

        return left.Weight < right.Weight;

    }
    public static bool operator >(Node left, Node right)
    {

        return left.Weight > right.Weight;

    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Node other = (Node)obj;
        return _x == other._x && _y == other._y;
    }
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + _x.GetHashCode();
            hash = hash * 31 + _y.GetHashCode();
            return hash;
        }
    }

    public bool IsDestination { get => this._x == destination.Item1 && this._y == destination.Item2; }

    float Weight { get => _distanceFromOrigin + _distanceToDestination; }
    public int X { get => this._x; }
    public int Y { get => this._y; }
}



public class PathFinding
{
    public enum Types
    {

        SpatialAware,

        Regular,
    }
    Vector2 destination;
    Vector2 origin;

    Cell originCell;
    Cell destinationCell;

    Node originNode;

    List<Node> visitedNodes;
    HashSet<Node> waitingNodes;

    private LinkedList<Cell> cellPath;

    public LinkedList<Cell> CellPath { get => cellPath; }
    public Vector2 Destination { set => destination = value; }
    public Vector2 Origin { set => origin = value; }
    public static Node[] ObstacleNodes { get => obstacleNodes; }

    public Cell[] cellPathArray;

    static Node[] obstacleNodes;

    Pathfinder pathfinder;

    public PathFinding(Types type = Types.Regular, Vector2? scale = null)
    {
        visitedNodes = new List<Node>();

        waitingNodes = new HashSet<Node>();

        switch (type)
        {

            case Types.SpatialAware:

                pathfinder = new SpatialAwarePathfinder(obstacleNodes, visitedNodes, waitingNodes, scale);
                break;
            default:
                pathfinder = new Pathfinder  (obstacleNodes, visitedNodes, waitingNodes);
                break;

        }


    }

    public static void SetObstacleNodes()
    {

        float cameraHeight = -Camera.main.orthographicSize;

        float cameraWidth = cameraHeight * Camera.main.aspect;

        Cell firstCell = GridScript.GetCellCoords(new Vector2(cameraWidth, cameraHeight));

        int numberOfHorizontalCells = Mathf.Abs(firstCell.x * 2);
        int numberOfVerticalCells = Mathf.Abs(firstCell.y * 2);

        obstacleNodes = new Node[numberOfHorizontalCells * numberOfVerticalCells];

        int leftSide = firstCell.x;
        int rightSide = Mathf.Abs(firstCell.x) - 1;


        int lowerSide = firstCell.y;
        int upperSide = Mathf.Abs(firstCell.y) - 1;

        //Debug.Log(obstacleNodes.Count());


        for (int i = leftSide; i < rightSide; i++)
        {


            int fixedHorizontalIndex = Mathf.Abs(leftSide) + i;

            //Debug.Log(fixedHorizontalIndex + "-");

            fixedHorizontalIndex *= numberOfVerticalCells;


            for (int j = lowerSide; j < upperSide; j++)
            {
                Cell newCell = new Cell(i, j);

                int fixedVerticalIndex = Mathf.Abs(lowerSide) + j;

                int currentIndex = fixedHorizontalIndex + fixedVerticalIndex;


                if (GridScript.IsCellOccupied(newCell))
                {

                    Node cellNode = new Node(i, j);

                    obstacleNodes[currentIndex] = cellNode;
                }

            }


        }


    }

    public LinkedList<Cell> GetPath(Vector2 from, Vector2 to)
    {

        visitedNodes = new List<Node>();
        
        //waitingNodes = new HashSet<Node>();
        
        waitingNodes.Clear();

        originCell = GridScript.GetCellCoords(from);

        destinationCell = GridScript.GetCellCoords(to);

        Node.Destination = (destinationCell.x, destinationCell.y);

        originNode = new Node(originCell.x, originCell.y);

        LinkedList<Node> path = AStarPathFinding();

        cellPath = GetCellsFromNodes(path);

        cellPathArray = new Cell[cellPath.Count];

        cellPath.CopyTo(cellPathArray, 0);
        return cellPath;

    }




    LinkedList<Node> AStarPathFinding()
    {
        Node currentNode = originNode;
        int counter = 0;

        while (!currentNode.IsDestination)
        {

            counter++;

            visitedNodes.Add(currentNode);
            waitingNodes.Remove(currentNode);

            List<Node> neighbours = pathfinder.GetNeighbours(currentNode);

            pathfinder.PruneNeighbours(neighbours);

            currentNode = pathfinder.GetBestFromWaitingNodes();
        }

        visitedNodes.Add(currentNode);

        return currentNode.Path;

    }

    LinkedList<Cell> GetCellsFromNodes(LinkedList<Node> nodes)
    {

        LinkedList<Cell> cells = new LinkedList<Cell>();

        foreach (Node currentNode in nodes)
        {
            cells.AddLast(new Cell { x = currentNode.X, y = currentNode.Y });

        }


        return cells;

    }
}