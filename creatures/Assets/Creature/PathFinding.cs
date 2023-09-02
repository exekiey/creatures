using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class PathFinding
{

    Vector2 destination;
    Vector2 origin;

    Cell originCell;
    Cell destinationCell;

    Node originNode;

    List<Node> visitedNodes;
    HashSet<Node> waitingNodes;

    private LinkedList<Cell> cellPath;

    public LinkedList<Cell> CellPath { get => cellPath;}
    public Vector2 Destination { set => destination = value; }
    public Vector2 Origin { set => origin = value; }

    public Cell[] cellPathArray;

    static Node[] obstacleNodes;

    class Node
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

            _x = x;
            _y = y;

            _parent = parent; 

            _distanceFromOrigin = distanceFromParent + parent._distanceFromOrigin;

            _distanceToDestination = Mathf.Sqrt( Mathf.Pow(_x - destination.Item1, 2) + Mathf.Pow(_y - destination.Item2, 2));

        }
        public Node(int x, int y)
        {

            _x = x;
            _y = y;

            _distanceFromOrigin = 0;

            _distanceToDestination = Mathf.Sqrt(Mathf.Pow(_x - destination.Item1, 2) + Mathf.Pow(_y - destination.Item2, 2));

        }

        public override string ToString()
        {
            return _x.ToString() + ":" + _y.ToString();
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

        public List<Node> Neighbours
        {
            get
            {

                float squareOfTwo = 1.414f;

                List<Node> neighbours = new List<Node>();

                Node up = new Node(_x, _y + 1, this, 1);
                neighbours.Add(up);

                Node upLeft = new Node(_x - 1, _y + 1, this, squareOfTwo);
                neighbours.Add(upLeft);

                Node upRight = new Node(_x + 1, _y + 1, this, squareOfTwo);
                neighbours.Add(upRight);

                Node down = new Node(_x, _y - 1, this, 1);
                neighbours.Add(down);

                Node downLeft = new Node(_x - 1, _y - 1, this, squareOfTwo);
                neighbours.Add(downLeft);

                Node downRight = new Node(_x + 1, _y - 1, this, squareOfTwo);
                neighbours.Add(downRight);

                Node left = new Node(_x - 1, _y, this, 1);
                neighbours.Add(left);

                Node right = new Node(_x + 1, _y, this, 1);
                neighbours.Add(right);

                return neighbours;

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
            if (obj is Node other)
            {

                return X == other.X && Y == other.Y;

            }
            return false;
        }

        public override int GetHashCode()
        {
            int numberOfZeroes = (int)Mathf.Log10(Y) + 1;

            int res = X * (int)Mathf.Pow((float)10, (float)numberOfZeroes) + Y;

            return res;
        }

        public bool IsDestination { get => _x == destination.Item1 && _y == destination.Item2; }

        float Weight { get => _distanceFromOrigin + _distanceToDestination; }
        public int X { get => _x;}
        public int Y { get => _y;}
    }

    public static void SetObstacleNodes()
    {

        float cameraHeight = -Camera.main.orthographicSize;

        float cameraWidth = cameraHeight * Camera.main.aspect;

        Cell firstCell = GridScript.GetCellCoords(new Vector2(cameraWidth, cameraHeight));

        int numberOfHorizontalCells = Mathf.Abs(firstCell.x * 2);
        int numberOfVerticalCells = Mathf.Abs(firstCell.y * 2);

        obstacleNodes = new Node[numberOfHorizontalCells * numberOfVerticalCells];

        Debug.Log(obstacleNodes.Count());

        int leftSide = firstCell.x;
        int rightSide = Mathf.Abs(firstCell.x) - 1;

        int lowerSide = firstCell.y;
        int upperSide = Mathf.Abs(firstCell.y) - 1;

        for (int i = leftSide; i < rightSide; i++)
        {

            for (int j = lowerSide; j < upperSide; j++)
            {
                Cell newCell = new Cell(i, j);

                int currentIndex = (Mathf.Abs(leftSide) + i) * numberOfHorizontalCells + (Mathf.Abs(lowerSide) + j);

                Debug.Log(Mathf.Abs(leftSide) + i);
               
                if (GridScript.IsCellOccupied(newCell))
                {
                    obstacleNodes[currentIndex] = new Node(i, j);
                }

            }

        }

        foreach(Node node in obstacleNodes)
        {

            if (node != null)
            {

                Debug.Log(node.ToString());

            }

        }

    }

    public LinkedList<Cell> GetPath(Vector2 from, Vector2 to)
    {


        visitedNodes = new List<Node>();

        originCell = GridScript.GetCellCoords(from);

        destinationCell = GridScript.GetCellCoords(to);

        Node.Destination = (destinationCell.x, destinationCell.y);
        originNode = new Node(originCell.x, originCell.y);

        visitedNodes = new List<Node>();
        waitingNodes = new HashSet<Node>();

        LinkedList<Node> path = APathFinding();

        cellPath = GetCellsFromNodes(path);

        cellPathArray = new Cell[cellPath.Count];

        cellPath.CopyTo(cellPathArray, 0);
        
        return cellPath;

    }

    Node GetBestNode()
    {


        Node bestNode = waitingNodes.First();

        foreach(Node currentNode in waitingNodes)
        {

            if (bestNode > currentNode)
            {
                bestNode = currentNode;
            }

        }

        return bestNode;

    }


    LinkedList<Node> APathFinding()
    {

        Node currentNode = originNode;

        while (!currentNode.IsDestination)
        {
            visitedNodes.Add(currentNode);
            waitingNodes.Remove(currentNode);

            List<Node> neighbours = currentNode.Neighbours;


            foreach (Node currentBestNode in neighbours)
            {

                if (obstacleNodes.Contains(currentBestNode)) continue;

                waitingNodes.Add(currentBestNode);

            }

            currentNode = GetBestNode();
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


    public void DrawPath()
    {


    }
}
