using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class SpatialAwarePathfinder : Pathfinder
{

    Vector2 _size;

    Node[] unfittingNodes;

    public static Vector2 PivotPosition(GameObject gameObject)
    {

        Vector2 pivot;
        pivot = gameObject.transform.position;

        /*
        pivot = GridScript.RoundToCell(gameObject.transform.position);

        pivot = gameObject.transform.position;

        Vector2 objectScaleInCells = GridScript.GetSizeInCells(gameObject.transform.lossyScale);

        Vector2 fixedScale = (objectScaleInCells - Vector2.one) * 0.5f;


        fixedScale = new Vector2(Mathf.Floor(fixedScale.x), Mathf.Floor(fixedScale.y));

        fixedScale *= GridScript.GridSize;

        pivot += fixedScale;
        */

        pivot += (Vector2) gameObject.transform.lossyScale / 2;
        pivot.x -= GridScript.GridSize / 2;
        pivot.y -= GridScript.GridSize / 2;

        return pivot;


    }

    public SpatialAwarePathfinder(Node[] obstacleNodes, List<Node> visitedNodes, HashSet<Node> waitingNodes, Vector2? size) : base (obstacleNodes, visitedNodes, waitingNodes)
    {

        if (!size.HasValue)
        {

            throw new ArgumentException("Size is required for SpatialAwarePathFinder");

        }

        _size = (Vector2) size;

        if (_size.x < 0 || _size.y < 0)
        {
            throw new ArgumentException("Size must be bigger than 0");
        }

        _size = GridScript.GetSizeInCells(_size);

        SetUnfittingNodes();

    }

    public void SetUnfittingNodes()
    {

        float cameraHeight = -Camera.main.orthographicSize;

        float cameraWidth = cameraHeight * Camera.main.aspect;

        Cell firstCell = GridScript.GetCellCoords(new Vector2(cameraWidth, cameraHeight));

        int numberOfHorizontalCells = Mathf.Abs(firstCell.x * 2);
        int numberOfVerticalCells = Mathf.Abs(firstCell.y * 2);

        unfittingNodes = new Node[numberOfHorizontalCells * numberOfVerticalCells];

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

                int fixedVerticalIndex = Mathf.Abs(lowerSide) + j;

                int currentIndex = fixedHorizontalIndex + fixedVerticalIndex;

                Node currentNode = new Node(i, j);

                if (obstacleNodes.Contains(currentNode))
                {
                    continue;
                }

                bool isSpaceBigEnough = IsSpaceBigEnough(currentNode);

                if (!isSpaceBigEnough)
                {
                    unfittingNodes[currentIndex] = currentNode;
                }

            }
        }

        foreach(Node currentNode in unfittingNodes)
        {
            /*
            if (currentNode != null)
            Debug.Log(currentNode.ToString());
            */
        }
    }

    private bool IsSpaceBigEnough(Node node)
    {


        for (int i = 1; i < _size.x; i++)
        {
            if (obstacleNodes.Contains(new Node(node.X - i, node.Y)))
            {

                return false;

            }


        }

        for (int i = 1; i < _size.y; i++)
        {

            if (obstacleNodes.Contains(new Node(node.X, node.Y - i)))
            {

                return false;

            }


        }


        return true;

    }

    public override void PruneNeighbours(List<Node> neighbours)
    {

        foreach (Node currentNeigbour in neighbours)
        {
            
            bool isCurrentNeighbourObstacle = obstacleNodes.Contains(currentNeigbour);

            if (isCurrentNeighbourObstacle) continue;

            bool isCurrentNeighbourFitting = unfittingNodes.Contains(currentNeigbour);

            if (isCurrentNeighbourFitting) continue;

            waitingNodes.Add(currentNeigbour);
        }


    }
}


