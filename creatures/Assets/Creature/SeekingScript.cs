using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeekingScript : MonoBehaviour
{
    [SerializeField] Vector2 targetPosition;
    [SerializeField] float moveForce;

    [SerializeField] public bool followCursor;
    [HideInInspector] public GameObject tarjetObject;

    Cell currentCell;

    [SerializeField] bool usePathFinding;

    Rigidbody2D _rigidbody2D;

    PathFinding _pathFinding;

    LinkedList<Cell> cellPath;

    Cell nextCell;

    Camera mainCam;

    Vector2 pivotPosition;

    public float MoveForce { get => moveForce; set => moveForce = value; }

    private void Start()
    {

        targetPosition = tarjetObject.transform.position;

        if (usePathFinding)
        {

            _pathFinding = new PathFinding(PathFinding.Types.SpatialAware, transform.lossyScale);

            //_rigidbody2D = pivot.AddComponent<Rigidbody2D>();
            _rigidbody2D = GetComponentInParent<Rigidbody2D>();

            //gameObject.transform.SetParent(pivot.transform);

            cellPath = _pathFinding.GetPath(SpatialAwarePathfinder.PivotPosition(gameObject), targetPosition);

            nextCell = cellPath.First();
            
            cellPath.RemoveFirst();

        }

        mainCam = Camera.main;

    }




    private void Update()
    {

        pivotPosition = SpatialAwarePathfinder.PivotPosition(gameObject);

        for (int i = 0; i < _pathFinding.cellPathArray.Length - 1; i++)
        {
            Vector2 from = GridScript.GetRealWorldCoords(_pathFinding.cellPathArray.ElementAt(i));
            Vector2 to = GridScript.GetRealWorldCoords(_pathFinding.cellPathArray.ElementAt(i + 1));

            Debug.DrawLine(from, to, Color.red);

        }
        /*
        GetComponents<PositionSquare>()[0].RealPosition = pivotPosition;
        GetComponents<PositionSquare>()[1].RealPosition = new Vector2(nextCell.x, nextCell.y);
        GetComponents<PositionSquare>()[1].Color = Color.yellow;
        */

    }

    private void FixedUpdate()
    {
        
        if (usePathFinding)
        {

            if (cellPath.Count > 0)
            {
                if (GridScript.GetCellCoords(pivotPosition) == nextCell)
                {
                    nextCell = cellPath.First();
                    cellPath.RemoveFirst();

                    targetPosition = GridScript.GetRealWorldCoords(nextCell);

                }
            }

        }

        Vector2 direction = (targetPosition - pivotPosition).normalized;

        _rigidbody2D.AddForceAtPosition(direction * moveForce, Vector2.zero);

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(pivotPosition, 0.1f);
    }
}
