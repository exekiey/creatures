using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Test : MonoBehaviour
{

    [SerializeField] GameObject test;

    [SerializeField] GameObject target;
    PathFinding pathFinding;
    LinkedList<Cell> cellPath;

    Cell[] cellPathArray;

    void Start()
    {
        pathFinding = new PathFinding(PathFinding.Types.SpatialAware, test.transform.lossyScale);

        cellPath = pathFinding.GetPath(test.transform.position, target.transform.position);

        Debug.Log(cellPath.Count);

        cellPathArray = cellPath.ToArray();

    }

    private void Update()
    {
        DrawPath();
    }

    void DrawPath()
    {


        for (int i = 0; i < pathFinding.cellPathArray.Length - 1; i++)
        {
            Vector2 from = GridScript.GetRealWorldCoords(pathFinding.cellPathArray.ElementAt(i));
            Vector2 to = GridScript.GetRealWorldCoords(pathFinding.cellPathArray.ElementAt(i + 1));

            Debug.DrawLine(from, to, Color.red);

        }

    }
}
