using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;



public class TentacleData
{
    public Grabable grabable;
    public float elapsedTime;

    public TentacleData(Grabable grabable, float elapsedTime)
    {
        this.grabable = grabable;
        this.elapsedTime = elapsedTime;
    }
}

public class TentaclePathfindingPlacer : MonoBehaviour
{
    /*
    abstract class TentacleState
    {

        protected TentaclePathFinding _tentacle;
        protected TentaclePathfindingPlacer _context;

        public TentacleState(TentaclePathFinding tentacle, TentaclePathfindingPlacer context)
        {
            _tentacle = tentacle;
            _context = context;
        }

        public abstract void EnterState(params object[] parameters);
        public abstract void UpdateState();
        public abstract void FixedUpdateState();
        public abstract void ExitState();


    }

    class HangingState : TentacleState
    {

        public HangingState(TentaclePathFinding tentacle, TentaclePathfindingPlacer context) : base(tentacle, context)
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
        public PluggedState(TentaclePathFinding tentacle, TentaclePathfindingPlacer context) : base(tentacle, context)
        {
        }

        public override void EnterState(params object[] parameters)
        {
        }
        public override void UpdateState()
        {
            bool isTooStreched = (_tentacle.Tentacle.CurrentTotalLength > _tentacle.Tentacle.TentacleLength + _tentacle.Tentacle.Loseness) && _tentacle.Tentacle.IsColliding;

            bool isTooSeparated = (_tentacle.Tentacle.Separation > _tentacle.Tentacle.TentacleLength + _tentacle.Tentacle.Loseness) && !_tentacle.Tentacle.IsColliding;

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


        public MovingState(TentaclePathFinding tentacle, TentaclePathfindingPlacer context) : base(tentacle, context)
        {

            PathFinding pathFinding = new PathFinding();

            Grabable grabable = Grabable.ClosestGrabable(_tentacle.transform.position);

            if (grabable == null)
            {

                _context.SwitchState(State.Hanging);

            }

            pathFinding.GetPath(_tentacle.Tentacle.Points.First().currentPosition, grabable.transform.position);

        }

        public override void EnterState(params object[] parameters)
        {



            parameters[0] = cellPath;

            currentCell = cellPath.First();

            cellPath.RemoveFirst();

            nextCell = cellPath.First();

            lastCell = cellPath.Last();
        }
        public override void UpdateState()
        {

            if (currentCell != lastCell)
            {

                currentCell = GridScript.GetCellCoords(_tentacle.transform.position);

                if (currentCell == nextCell)
                {
                    cellPath.RemoveFirst();
                    nextCell = cellPath.First();
                    nextPosition = GridScript.GetRealWorldCoords(nextCell);
                }

                Vector2 currentPosition = _tentacle.Tentacle.Last.currentPosition;

                Vector2 direction = (nextPosition - currentPosition).normalized;

                _tentacle.Tentacle.Last.currentPosition += direction * _tentacle.MoveForce;

            }
            else
            {



            }


        }
        public override void FixedUpdateState()
        {
        }
        public override void ExitState()
        {
        }
    }


    TentaclePathFinding[] tentaclePathFindings;
    Dictionary<TentaclePathFinding, TentacleData> attachedGrabables;
    SeekingScript parentSeekingScript;

    HangingState hanging;
    MovingState moving;
    PluggedState plugged;
    TentacleState currentState;

    [SerializeField] float maxMoveForce;
    [SerializeField] float maxGravity;

    [SerializeField] float updateGrababablesTime;
    float updateTimer;
    
    float gravityContribution;
    float moveForceContribution;
    private int waitingCounter;

    private void Awake()
    {
        parentSeekingScript = GetComponentInParent<SeekingScript>();
    }

    void Start()
    {

        parentSeekingScript.MoveForce = maxMoveForce;

        tentaclePathFindings = GetComponentsInChildren<TentaclePathFinding>();

        attachedGrabables = new Dictionary<TentaclePathFinding, TentacleData>(tentaclePathFindings.Count());

        foreach (TentaclePathFinding tentaclePathFinding in tentaclePathFindings)
        {

            Grabable closestGrabable = Grabable.ClosestGrabable(transform.position);
            if (closestGrabable != null)
            {
                tentaclePathFinding.SetTarget(closestGrabable.gameObject);

                attachedGrabables[tentaclePathFinding] = new TentacleData(closestGrabable, 0);

            }
        }

        gravityContribution = maxGravity / tentaclePathFindings.Length;
        moveForceContribution = maxMoveForce / tentaclePathFindings.Length;

    }

    public void SwitchState(State state)
    {

        switch (state)
        {

            case State.Moving:
                moving.ExitState();

                currentState = moving;
                moving.EnterState();
                break;
            case State.Plugged:
                plugged.ExitState();

                currentState = plugged;
                plugged.EnterState();
                break;
            case State.Hanging:
                hanging.ExitState();

                currentState = hanging;
                hanging.EnterState();
                break;

        }

    }

    private void SetGrabable(TentaclePathFinding tentaclePathFinding, Grabable grabable = null)
    {
        Grabable closestGrabable = Grabable.ClosestGrabable(transform.position, grabable);

        if (closestGrabable != null)
        {
            tentaclePathFinding.SetTarget(closestGrabable.gameObject);

            attachedGrabables[tentaclePathFinding].grabable = closestGrabable;

        }
    }




    private void Update()
    {
        
        if (updateTimer > updateGrababablesTime)
        {
            Grabable.ResetGrabables();
            //FindGrabablesAndAttach();
            updateTimer = 0;
        } else
        {
            updateTimer += Time.deltaTime;
        }
        

        HandleAttachments();

        ComputeGravityAndForce();

    }

    private void HandleAttachments()
    {


    }

    private void ComputeGravityAndForce()
    {
        float computedGravity = maxGravity;
        float computedMoveForce = 0;

        int counter = 0;

        foreach (TentaclePathFinding tentaclePathFinding in tentaclePathFindings)
        {

            counter++;
            if (tentaclePathFinding.IsPlugged)
            {
                computedGravity -= gravityContribution;
                computedMoveForce += moveForceContribution;
            }

        }
        
        parentSeekingScript.GetComponent<Rigidbody2D>().gravityScale = computedGravity;
        parentSeekingScript.MoveForce = computedMoveForce;
    }
    */
}
