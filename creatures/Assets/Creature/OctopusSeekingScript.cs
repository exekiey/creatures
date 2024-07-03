using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OctopusSeekingScript : MonoBehaviour
{
    [SerializeField] Vector2 targetPosition;
    [SerializeField] float moveForce;
    [SerializeField] PreyScript tarjetObject;

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


        pivotPosition = SpatialAwarePathfinder.PivotPosition(gameObject);

        if (usePathFinding)
        {

            _pathFinding = new PathFinding(PathFinding.Types.SpatialAware, transform.lossyScale);

            //_rigidbody2D = pivot.AddComponent<Rigidbody2D>();
            _rigidbody2D = GetComponentInParent<Rigidbody2D>();

            //gameObject.transform.SetParent(pivot.transform);

            cellPath = _pathFinding.GetPath(pivotPosition, tarjetObject.transform.position);

            nextCell = cellPath.First();

            targetPosition = GridScript.GetRealWorldCoords(nextCell);

            cellPath.RemoveFirst();

        }

        mainCam = Camera.main;

    }

    private void OnEnable()
    {
        tarjetObject.changedCell += Reroute;
    }

    private void OnDisable()
    {
        tarjetObject.changedCell -= Reroute;
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

    bool ApproximatelyVector2(Vector2 a, Vector2 b, float threshold)
    {

        float xDiff = Mathf.Abs(a.x - b.x);
        float yDiff = Mathf.Abs(a.y - b.y);

        bool xEquals = xDiff <= threshold;
        bool yEquals = yDiff <= threshold;

        return xEquals && yEquals;

    }

    private void FixedUpdate()
    {
        
        if (usePathFinding)
        {

            if (cellPath.Count > 0)
            {
                if (ApproximatelyVector2(pivotPosition, targetPosition, 0.01f))
                {

                    nextCell = cellPath.First();
                    cellPath.RemoveFirst();

                    targetPosition = GridScript.GetRealWorldCoords(nextCell);

                }
            }

        }

        Vector2 direction = (targetPosition - pivotPosition).normalized;

        //_rigidbody2D.AddForceAtPosition(direction * moveForce, Vector2.zero);
        _rigidbody2D.AddForce(direction * moveForce);
        _rigidbody2D.velocity = direction * moveForce;

    }

    void Reroute()
    {
        cellPath = _pathFinding.GetPath(pivotPosition, tarjetObject.transform.position);

        nextCell = cellPath.First.Next.Value;

        targetPosition = GridScript.GetRealWorldCoords(nextCell);

        cellPath.RemoveFirst();

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(pivotPosition, 0.1f);
    }
}
