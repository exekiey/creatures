using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
public enum State
{
    Hanging,
    Plugged,
    Moving
}
abstract class TentacleState
{

    protected TentaclePathFinding _context;

    public TentacleState(TentaclePathFinding context)
    {
        _context = context;
    }

    public abstract void EnterState(params object[] parameters);
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract void ExitState();


}

class HangingState : TentacleState
{

    public HangingState(TentaclePathFinding context) : base(context)
    {
    }

    public override void EnterState(params object[] parameters)
    {
    }
    public override void UpdateState()
    {
    }
    public override void FixedUpdateState()
    {
    }
    public override void ExitState()
    {
    }

}

class PluggedState : TentacleState
{
    public PluggedState(TentaclePathFinding context) : base(context)
    {
    }

    public override void EnterState(params object[] parameters)
    {

        _context.ContributeForcePositive();
        _context.ContributeGravityNegative();
        _context.Tentacle.Last.locked = true;
    }
    public override void UpdateState()
    {
        bool isTooStreched = (_context.Tentacle.CurrentTotalLength > _context.Tentacle.TentacleLength + _context.Tentacle.Loseness) && _context.Tentacle.IsColliding;

        bool isTooSeparated = (_context.Tentacle.Separation > _context.Tentacle.TentacleLength + _context.Tentacle.Loseness) && !_context.Tentacle.IsColliding;

        if (isTooSeparated || isTooStreched)
        {
            _context.SwitchState(State.Moving);
        }


    }
    public override void FixedUpdateState()
    {
    }
    public override void ExitState()
    {
        _context.Tentacle.Last.locked = false;
        _context.AttachedGrabable.Deselect();
        _context.ContributeForceNegative();
        _context.ContributeGravityPositive();
    }


}

class MovingState : TentacleState
{

    LinkedList<Cell> cellPath;

    Cell currentCell;
    Cell nextCell;
    Vector2 nextPosition;

    Cell lastCell;

    PathFinding pathFinding;

    float tooLongWaitingCounter;

    public MovingState(TentaclePathFinding context) : base(context)
    {
        tooLongWaitingCounter = 0;
    }

    public override void EnterState(params object[] parameters)
    {
        FindGrabable();
    }

    private void FindGrabable()
    {
        pathFinding = new PathFinding();

        Grabable grabable = Grabable.ClosestGrabable(_context.Body.transform.position);


        if (grabable == null)
        {

            _context.SwitchState(State.Hanging);
            return;
        }

        _context.AttachedGrabable = grabable;

        cellPath = pathFinding.GetPath(_context.Tentacle.Points.First().currentPosition, grabable.transform.position);

        currentCell = cellPath.First();

        nextCell = cellPath.First.Next.Value;

        nextPosition = GridScript.GetRealWorldCoords(nextCell);

        lastCell = cellPath.Last();
    }

    public override void UpdateState()
    {

        currentCell = GridScript.GetCellCoords(_context.Tentacle.Last.currentPosition);

        if (currentCell != lastCell)
        {


            if (cellPath.Contains(currentCell))
            {
                nextCell = cellPath.Find(currentCell).Next.Value;
                nextPosition = GridScript.GetRealWorldCoords(nextCell);
            } else
            {

                pathFinding = new PathFinding();

                cellPath = pathFinding.GetPath(_context.Tentacle.Last.currentPosition, _context.AttachedGrabable.transform.position);

            }



            Vector2 currentPosition = _context.Tentacle.Last.currentPosition;

            Vector2 direction = (nextPosition - currentPosition).normalized;

            _context.Tentacle.Last.currentPosition += direction * _context.MoveForce * Time.deltaTime;

        } else
        {

            _context.SwitchState(State.Plugged);

        }
        /*
        if (tooLongWaitingCounter > 5)
        {
            _context.AttachedGrabable.Deselect();
            FindGrabable();
        }*/
        tooLongWaitingCounter += Time.deltaTime;
    }


    public override void FixedUpdateState()
    {
    }
    public override void ExitState()
    {
    }
}

[RequireComponent(typeof(Tentacle))]
public class TentaclePathFinding : MonoBehaviour
{

    [SerializeField] GameObject target;
    [SerializeField] float moveForce;
    [SerializeField] OctopusSeekingScript body;
    TentaclePathFindingForceContributor forceContributor;

    HangingState hanging;
    MovingState moving;
    PluggedState plugged;
    TentacleState current;

    Tentacle tentacle;

    Grabable attachedGrabable;

    public float MoveForce { set => moveForce = value; get => moveForce; }
    public Tentacle Tentacle { get => tentacle; set => tentacle = value; }
    public Grabable AttachedGrabable { get => attachedGrabable; set => attachedGrabable = value; }
    public OctopusSeekingScript Body { get => body; }

    /*
    public void SetTarget(GameObject value)
    {

        tentacle.Points.Last().locked = true;

        target = value;

        targetPosition = target.transform.position;


        SetPath();
    }*/
    /*
    private void SetPath()
    {

        pathFinding = new PathFinding();

        tentacle = GetComponent<Tentacle>();

        cellPath = pathFinding.GetPath(tentacle.Points.Last().currentPosition, targetPosition);

        cellPath.RemoveFirst();

        lastCell = cellPath.Last();

        StartCoroutine(Move());

    }*/

    private void Awake()
    {
        tentacle = GetComponent<Tentacle>();
        forceContributor = transform.parent.GetComponent<TentaclePathFindingForceContributor>();
    }

    private void Start()
    {

        hanging = new HangingState(this);
        moving = new MovingState(this);
        plugged = new PluggedState(this);

        current = moving;
        current.EnterState();

    }

    /*
    IEnumerator Move()
    {

        float longCounter = 0;

        isMoving = true;

        Cell currentCell = GridScript.GetCellCoords(tentacle.Points.Last().currentPosition);
        Cell nextCell = cellPath.First();

        while (currentCell != lastCell )
        {

            currentCell = GridScript.GetCellCoords(tentacle.Points.Last().currentPosition);

            currentCellVector.x = currentCell.x;
            currentCellVector.y = currentCell.y;

            nextCellVector.x = nextCell.x;
            nextCellVector.y = nextCell.y;

            bool isTooStreched = tentacle.CurrentTotalLength > tentacle.TentacleLength + tentacle.Loseness;

            bool isTooSeparated = (tentacle.Separation > tentacle.TentacleLength + tentacle.Loseness) && !tentacle.IsColliding;

            bool isTooLong = isTooStreched || isTooSeparated;



            if (isTooLong)
            {
                tentacle.GetComponent<LineRenderer>().startColor = Color.yellow;
                tentacle.GetComponent<LineRenderer>().endColor = Color.yellow;

            } else
            {
                tentacle.GetComponent<LineRenderer>().startColor = Color.white;
                tentacle.GetComponent<LineRenderer>().endColor = Color.white;
            }

            if (isTooLong)
            {
                longCounter += Time.deltaTime;
            } else
            {
                longCounter = 0;
            }

            Vector2 nextCellCoords = GridScript.GetRealWorldCoords(nextCell);
            Vector2 currentCellCords = GridScript.GetRealWorldCoords(currentCell);

            nextDistance = Vector2.Distance(nextCellCoords, tentacle.Points.First().currentPosition);
            currentDistance = Vector2.Distance(currentCellCords, tentacle.Points.First().currentPosition);

            bool isNextCloser = currentDistance >= nextDistance;

            if (!isTooLong || isNextCloser)
            {

                if (currentCell == nextCell && nextCell != lastCell)
                {
                    nextCell = cellPath.First();
                    cellPath.RemoveFirst();

                    targetPosition = GridScript.GetRealWorldCoords(nextCell);

                }

                Vector2 direction = (targetPosition - tentacle.Points.Last().currentPosition).normalized;

                tentacle.Points.Last().currentPosition += direction * moveForce * Time.deltaTime;

            }

            yield return null;
        }

        isMoving = false;

        yield return null;

    }*/
    public void SwitchState(State state)
    {

        current.ExitState();
        switch (state)
        {

            case State.Moving:
                current = moving;
                break;
            case State.Plugged:
                current = plugged;
                break;
            case State.Hanging:
                current = hanging;
                break;

        }
        current.EnterState();

    }

    void Update()
    {
        current.UpdateState();

        /*
        bool isTooStreched = (tentacle.CurrentTotalLength > tentacle.TentacleLength + tentacle.Loseness) && tentacle.IsColliding;

        bool isTooSeparated = (tentacle.Separation > tentacle.TentacleLength+ tentacle.Loseness) && !tentacle.IsColliding;

        if ((isTooSeparated || isTooStreched) && isPlugged)
        {
            Unplug();
        }

        isPlugged = GridScript.GetCellCoords(tentacle.Points.Last().currentPosition) == lastCell;

        if (isPlugged)
        {
            tentacle.GetComponent<LineRenderer>().startColor = Color.red;
            tentacle.GetComponent<LineRenderer>().endColor = Color.red;
        } else
        {
            tentacle.GetComponent<LineRenderer>().startColor = Color.white;
            tentacle.GetComponent<LineRenderer>().endColor = Color.white;
        }*/
    }
    /*
    public void Unplug()
    {
        isPlugged = false;
        isMoving = false;
        tentacle.Points.Last().locked = false;
    }*/


    /*
    void DrawPath()
    {

        if (pathFinding != null)
        {
            for (int i = 0; i < pathFinding.cellPathArray.Length - 1; i++)
            {
                Vector2 from = GridScript.GetRealWorldCoords(pathFinding.cellPathArray.ElementAt(i));
                Vector2 to = GridScript.GetRealWorldCoords(pathFinding.cellPathArray.ElementAt(i + 1));

                Debug.DrawLine(from, to, Color.red);

            }
        }


    }
    */

    public void ContributeForcePositive()
    {
        forceContributor.ContributeForcePositive();
    }
    public void ContributeForceNegative()
    {
        forceContributor.ContributeForceNegative();
    }

    public void ContributeGravityPositive()
    {
        forceContributor.ContributeGravityPositive();
    }
    public void ContributeGravityNegative()
    {
        forceContributor.ContributeGravityNegative();
    }

}
