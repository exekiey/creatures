using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Tentacle))]
[RequireComponent(typeof(PathFinding))]
public class TentaclePathFinding : MonoBehaviour
{

    [SerializeField] GameObject target;
    [SerializeField] float moveForce;

    Vector2 targetPosition;

    Tentacle tentacle;

    PathFinding pathFinding;

    LinkedList<Cell> cellPath;

    Cell nextCell;

    // Start is called before the first frame update
    void Awake()
    {
        tentacle = GetComponent<Tentacle>();

        pathFinding = GetComponent<PathFinding>();

        targetPosition = target.transform.position;





    }

    private void Start()
    {

        pathFinding.Origin = tentacle.Points.Last().currentPosition;
        pathFinding.Destination = targetPosition;

        cellPath = pathFinding.GetPath();

        nextCell = cellPath.First();

        cellPath.RemoveFirst();

        tentacle.Points.Last().locked = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (cellPath.Count > 0)
        {
            if (GridScript.GetCellCoords(tentacle.Points.Last().currentPosition) == nextCell)
            {
                nextCell = cellPath.First();
                cellPath.RemoveFirst();

                targetPosition = GridScript.GetRealWorldCoords(nextCell);

            }
        }

        Vector2 direction = (targetPosition - tentacle.Points.Last().currentPosition).normalized;


        if (tentacle.CurrentTotalDistance < tentacle.TentacleLength + 0.1f)
        {
            tentacle.Points.Last().currentPosition += direction * moveForce;
        }


    }
}
