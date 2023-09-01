using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;




public class PathFinding : MonoBehaviour
{

    Vector2 destination;
    Vector2 origin;

    Cell originCell;
    Cell destinationCell;

    Node originNode;

    List<Node> visitedNodes;
    private LinkedList<Cell> cellPath;

    public LinkedList<Cell> CellPath { get => cellPath;}
    public Vector2 Destination { set => destination = value; }
    public Vector2 Origin { set => origin = value; }

    Cell[] cellPathArray;

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

                    Debug.Log(currentNode.X + ":" + currentNode.Y); 

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

        public bool IsDestination { get => _x == destination.Item1 && _y == destination.Item2; }

        float Weight { get => _distanceFromOrigin + _distanceToDestination; }
        public int X { get => _x;}
        public int Y { get => _y;}
    }

    // Start is called before the first frame update
    void Awake()
    {
    }

    public LinkedList<Cell> GetPath()
    {


        visitedNodes = new List<Node>();

        originCell = GridScript.GetCellCoords(origin);

        destinationCell = GridScript.GetCellCoords(destination);

        Node.Destination = (destinationCell.x, destinationCell.y);
        originNode = new Node(originCell.x, originCell.y);

        visitedNodes = new List<Node>();

        LinkedList<Node> path = APathFinding();

        cellPath = GetCellsFromNodes(path);

        cellPathArray = new Cell[cellPath.Count];

        cellPath.CopyTo(cellPathArray, 0);

        return cellPath;

    }

    List<Node> PruneNodes(List<Node> nodes)
    {



        List<Node> prunedNodes = new List<Node>();

        foreach (Node currentNode in nodes)
        {


            if (visitedNodes.Contains(currentNode)) continue;

            Cell currentCell = new Cell { x = currentNode.X, y = currentNode.Y };

            if (GridScript.IsCellOccupied(currentCell)) continue;

            prunedNodes.Add(currentNode);

        }

        return prunedNodes;

    }

    Node GetSmallestNode(List<Node> neighbours)
    {


        Node smallestNode = neighbours[0];

        foreach(Node currentNode in neighbours)
        {

            if (smallestNode > currentNode)
            {
                smallestNode = currentNode;
            }

        }

        return smallestNode;

    }


    LinkedList<Node> APathFinding()
    {

        Node currentNode = originNode;

        while (!currentNode.IsDestination)
        {

            visitedNodes.Add(currentNode);

            List<Node> neighbours = currentNode.Neighbours;

            List<Node> prunedNeighbours = PruneNodes(neighbours);

            Node bestNeighbour = GetSmallestNode(prunedNeighbours);

            currentNode = bestNeighbour;
        }

        visitedNodes.Add(currentNode);

        return currentNode.Path;

    }


    LinkedList<Cell> GetCellsFromNodes(LinkedList<Node> nodes)
    {

        LinkedList<Cell> cells = new LinkedList<Cell>();

        foreach (Node currentNode in nodes)
        {
            //Debug.Log(currentNode.ToString());
            cells.AddLast(new Cell { x = currentNode.X, y = currentNode.Y });

        }


        return cells;

    }


    void DrawPath(Cell[] path)
    {

        for (int i = 0; i < path.Length - 1; i++)
        {

            Vector2 from = GridScript.GetRealWorldCoords(path.ElementAt(i));
            Vector2 to = GridScript.GetRealWorldCoords(path.ElementAt(i+1));

            Debug.DrawLine(from, to);

        }

    }

    private void Update()
    {
        if (cellPath != null)
        {
            DrawPath(cellPathArray);

        }

    }
}
