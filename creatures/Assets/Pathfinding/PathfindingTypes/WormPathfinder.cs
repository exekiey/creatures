using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormPathfinder : Pathfinder
{
    // Start is called before the first frame update

    public WormPathfinder(Node[] obstacleNodes, List<Node> visitedNodes, HashSet<Node> waitingNodes, Vector2? size) : base(obstacleNodes, visitedNodes, waitingNodes)
    {



    }


    public override List<Node> GetNeighbours(Node node)
    {

        return null;


    }

}
