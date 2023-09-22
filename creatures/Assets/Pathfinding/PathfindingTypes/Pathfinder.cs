using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder 
{

    protected Node[] obstacleNodes;
    protected List<Node> visitedNodes;
    protected HashSet<Node> waitingNodes;

    public Pathfinder(Node[] obstacleNodes, List<Node> visitedNodes, HashSet<Node> waitingNodes)
    {
        this.obstacleNodes = obstacleNodes;
        this.visitedNodes = visitedNodes;
        this.waitingNodes = waitingNodes;


    }

    virtual public void PruneNeighbours(List<Node> neighbours)
    {
        
        foreach (Node currentNeigbour in neighbours)
        {

            bool isCurrentNeighbourObstacle = obstacleNodes.Contains(currentNeigbour);
         
            if (isCurrentNeighbourObstacle) continue;

            waitingNodes.Add(currentNeigbour);
        }

    }

    virtual public Node GetBestFromWaitingNodes()
    {

        Node bestNode = waitingNodes.First();

        foreach (Node currentNode in waitingNodes)
        {

            if (bestNode > currentNode)
            {
                bestNode = currentNode;
            }

        }

        return bestNode;

    }

    virtual public List<Node> GetNeighbours(Node node)
    {


        List<Node> neighbours = new List<Node>();

        Node up = new Node(node.X, node.Y + 1, node, 1);
        neighbours.Add(up);

        Node down = new Node(node.X, node.Y - 1, node, 1);
        neighbours.Add(down);

        Node left = new Node(node.X - 1, node.Y, node, 1);
        neighbours.Add(left);

        Node right = new Node(node.X + 1, node.Y, node, 1);
        neighbours.Add(right);


        //diagonals
        /*
        float squareOfTwo = 1.414f;
        Node downLeft = new Node(node.X - 1, node.Y - 1, node, squareOfTwo);
        neighbours.Add(downLeft);

        Node downRight = new Node(node.X + 1, node.Y - 1, node, squareOfTwo);
        neighbours.Add(downRight);

        Node upLeft = new Node(node.X - 1, node.Y + 1, node, squareOfTwo);
        neighbours.Add(upLeft);

        Node upRight = new Node(node.X + 1, node.Y + 1, node, squareOfTwo);
        neighbours.Add(upRight);
        */

        return neighbours;
    }
}
