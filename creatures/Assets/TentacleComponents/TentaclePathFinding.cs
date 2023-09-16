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
    }


}

class MovingState : TentacleState
{

    LinkedList<Cell> cellPath;

    Cell currentCell;
    Cell nextCell;
    Vector2 nextPosition;

    Cell lastCell;

    PathFinding pathFinding = new PathFinding();

    public MovingState(TentaclePathFinding context) : base(context)
    {

    }

    public override void EnterState(params object[] parameters)
    {

        Grabable grabable = Grabable.ClosestGrabable(_context.transform.position);

        Debug.Log(grabable);

        if (grabable == null)
        {

            _context.SwitchState(State.Hanging);
            return;
        }

        cellPath = pathFinding.GetPath(_context.Tentacle.Points.First().currentPosition, grabable.transform.position);

        currentCell = cellPath.First();

        cellPath.RemoveFirst();

        nextCell = cellPath.First();

        lastCell = cellPath.Last();
    }
    public override void UpdateState()
    {

        if (currentCell != lastCell)
        {

            currentCell = GridScript.GetCellCoords(_context.transform.position);

            if (currentCell == nextCell)
            {
                cellPath.RemoveFirst();
                nextCell = cellPath.First();
                nextPosition = GridScript.GetRealWorldCoords(nextCell);
            }

            Vector2 currentPosition = _context.Tentacle.Points.Last().currentPosition;

            Vector2 direction = (nextPosition - currentPosition).normalized;

            _context.Tentacle.Points.Last().currentPosition += direction * _context.MoveForce;

        } else
        {

            _context.SwitchState(State.Plugged);

        }
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

    HangingState hanging;
    MovingState moving;
    PluggedState plugged;
    TentacleState current;

    Vector2 targetPosition;

    Tentacle tentacle;

    PathFinding pathFinding;

    LinkedList<Cell> cellPath;

    Cell lastCell;

    [SerializeField] Vector2 nextCellVector;
    [SerializeField] Vector2 currentCellVector;

    bool isPlugged;
    private bool isMoving;
    //temp
    [SerializeField] float nextDistance;
    [SerializeField] float currentDistance;

    public float MoveForce { set => moveForce = value; get => moveForce; }
    public bool IsPlugged { get => isPlugged; }
    public bool IsMoving { get => isMoving; }
    public Tentacle Tentacle { get => tentacle; set => tentacle = value; }

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

    }

    private void Start()
    {

        hanging = new HangingState(this);
        moving = new MovingState(this);
        plugged = new PluggedState(this);

        SwitchState(State.Moving);

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

        switch (state)
        {

            case State.Moving:
                moving.ExitState();
                current = moving;
                moving.EnterState();
                break;
            case State.Plugged:
                plugged.ExitState();
                current = plugged;
                plugged.EnterState();
                break;
            case State.Hanging:
                hanging.ExitState();
                current = hanging;
                hanging.EnterState();
                break;

        }

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
}
