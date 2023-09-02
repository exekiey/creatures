using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Tentacle))]
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

        pathFinding = new PathFinding();

        targetPosition = target.transform.position;

    }

    private void Start()
    {

        cellPath = pathFinding.GetPath(tentacle.Points.Last().currentPosition, targetPosition);

        /*
        foreach(Cell cell in cellPath)
        {
            Debug.Log(cell.x + " " + cell.y);
        }
        */
        nextCell = cellPath.First();

        cellPath.RemoveFirst();

        tentacle.Points.Last().locked = true;
    }

    void DrawPath()
    {

        for (int i = 0; i < pathFinding.cellPathArray.Length - 1; i++)
        {

            Vector2 from = GridScript.GetRealWorldCoords(pathFinding.cellPathArray.ElementAt(i));
            Vector2 to = GridScript.GetRealWorldCoords(pathFinding.cellPathArray.ElementAt(i + 1));

            Debug.DrawLine(from, to);

        }

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

        DrawPath();


        Vector2 direction = (targetPosition - tentacle.Points.Last().currentPosition).normalized;


        if (tentacle.CurrentTotalDistance < tentacle.TentacleLength + 0.1f)
        {
            tentacle.Points.Last().currentPosition += direction * moveForce;
        }


    }
}
