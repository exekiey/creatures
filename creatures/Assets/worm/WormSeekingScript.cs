using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro.EditorUtilities;
using UnityEngine;


public enum WormStateType
{

    RetractingState,
    ExtendingState,
    IddleState

}

abstract class WormState
{

    protected WormSeekingScript _context;

    public WormState(WormSeekingScript context)
    {
        _context = context;
    }

    public virtual void EnterState() {}
    public virtual void UpdateState(){}
    public virtual void LateUpdateState() {}
    public virtual void FixedUpdateState(){}
    public virtual void ExitState(){}

}


class IddleState : WormState
{

    public IddleState(WormSeekingScript context) : base(context) { }
    public override void EnterState()
    {
    }
    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            _context.SwitchState(WormStateType.RetractingState);

        }

    }


    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
    }


}

class RetractingState : WormState
{

    Vector2 direction;
    Vector2 perpendicularDirection;

    float initialSeparation;

    Vector2[] initialPositions;

    Tentacle.Point firstOnThreshold;
    Tentacle.Point lastOnThreshold;

    public RetractingState(WormSeekingScript context, Vector2[] initialPositions, Tentacle.Point firstOnThreshold, Tentacle.Point lastOnThreshold) : base(context)
    {

        Debug.Log(_context.ThresholdPoint);

        this.initialPositions = initialPositions;

        this.firstOnThreshold = firstOnThreshold;
        this.lastOnThreshold = lastOnThreshold;


    }

    public override void EnterState()
    {

        _context.Tentacle.Gravity = 1;

        lastOnThreshold.locked = true;
        firstOnThreshold.locked = true;

        for (int i = _context.ThresholdPoint; i < _context.Tentacle.NumberOfPoints - _context.ThresholdPoint; i++)
        {
            initialPositions[i - _context.ThresholdPoint] = _context.Tentacle.Points[i].currentPosition;
        }

        direction = (_context.Tentacle.Last.currentPosition - _context.Tentacle.First.currentPosition).normalized;

        direction = direction.normalized;
        perpendicularDirection = Vector2.Perpendicular(direction);

        initialSeparation = _context.Tentacle.TentacleLength;


    }


    public float QuadraticFormula(float a, float b, float c, float x)
    {

        float powerOfTwo = Mathf.Pow((b-x*a), 2);

        return -(powerOfTwo) - c;

    } 

    public override void UpdateState()
    {

        float currentHeight = _context.test;

        float b = Mathf.Sqrt(currentHeight);
        float a = (b * 2) / initialSeparation;
        float c = (b * b) * -1;

        for (int i = _context.ThresholdPoint; i < _context.Tentacle.NumberOfPoints - _context.ThresholdPoint - 1; i++)
        {

            float tentacleStep = Mathf.InverseLerp(_context.ThresholdPoint, _context.Tentacle.NumberOfPoints - _context.ThresholdPoint, i);


            float tentaclePoint = Mathf.Lerp(0, _context.Tentacle.TentacleLength, tentacleStep);

            float currentPointPos = QuadraticFormula(a, b, c, tentaclePoint);

            _context.Tentacle.Points[i].currentPosition.y = initialPositions[i - _context.ThresholdPoint].y + perpendicularDirection.y * currentPointPos;
        }

        firstOnThreshold.currentPosition.x = initialPositions[0].x + currentHeight;

        if (_context.test > _context.SeparationThreshold)
        {

            _context.SwitchState(WormStateType.ExtendingState);

        }

    }

    public override void FixedUpdateState()
    {
    }
    public override void ExitState()
    {
    }
}
class ExtendingState : WormState
{
    Vector2 direction;
    Vector2 perpendicularDirection;

    float initialSeparation;

    Vector2[] initialPositions;

    Tentacle.Point firstOnThreshold;
    Tentacle.Point lastOnThreshold;
    public ExtendingState (WormSeekingScript context, Vector2[] initialPositions, Tentacle.Point firstOnThreshold, Tentacle.Point lastOnThreshold) : base(context)
    {

        Debug.Log(_context.ThresholdPoint);

        this.initialPositions = initialPositions;

        this.firstOnThreshold = firstOnThreshold;
        this.lastOnThreshold = lastOnThreshold;


    }

    public override void EnterState()
    {


        _context.Tentacle.Gravity = -1;



        /*
        lastOnThreshold.locked = true;
        firstOnThreshold.locked = true;

        for (int i = _context.ThresholdPoint; i < _context.Tentacle.NumberOfPoints - _context.ThresholdPoint; i++)
        {
            initialPositions[i - _context.ThresholdPoint] = _context.Tentacle.Points[i].currentPosition;
        }

        direction = (_context.Tentacle.Last.currentPosition - _context.Tentacle.First.currentPosition).normalized;

        direction = direction.normalized;
        perpendicularDirection = Vector2.Perpendicular(direction);

        initialSeparation = _context.Tentacle.TentacleLength;
        */
    }
    public float QuadraticFormula(float a, float b, float c, float x)
    {

        float powerOfTwo = Mathf.Pow((b - x * a), 2);

        return -(powerOfTwo) - c;

    }
    public override void UpdateState()
    {
        /*
        float currentHeight = _context.test;

        float b = Mathf.Sqrt(currentHeight);
        float a = (b * 2) / initialSeparation;
        float c = (b * b) * -1;

        for (int i = _context.ThresholdPoint; i < _context.Tentacle.NumberOfPoints - _context.ThresholdPoint - 1; i++)
        {

            float tentacleStep = Mathf.InverseLerp(_context.ThresholdPoint, _context.Tentacle.NumberOfPoints - _context.ThresholdPoint, i);


            float tentaclePoint = Mathf.Lerp(0, _context.Tentacle.TentacleLength, tentacleStep);

            float currentPointPos = QuadraticFormula(a, b, c, tentaclePoint);

            _context.Tentacle.Points[i].currentPosition.y = initialPositions[i - _context.ThresholdPoint].y + perpendicularDirection.y * currentPointPos;
        }

        lastOnThreshold.currentPosition.x += Time.deltaTime;
        */


    }


    public override void FixedUpdateState()
    {
    }

    public override void ExitState()
    {
        _context.Tentacle.First.locked = false;
    }

}




public class WormSeekingScript : MonoBehaviour
{

    WormState currentState;
    RetractingState retractingState;
    ExtendingState extendingState;
    IddleState iddleState;

    Tentacle tentacle;

    [SerializeField] float separationThreshold;
    [SerializeField] float extensionThreshold;
    [SerializeField] float moveForce;
    [SerializeField] [Range(0, 1)] float threshold;
    [SerializeField] public float test;

    public Tentacle Tentacle { get => tentacle; }
    public float MoveForce { get => moveForce; }
    public float SeparationThreshold { get => separationThreshold; set => separationThreshold = value; }
    public float Threshold { get => threshold; }
    public int ThresholdPoint { get => Mathf.FloorToInt(threshold * tentacle.NumberOfPoints); }

    private void Awake()
    {
        tentacle = GetComponent<Tentacle>();
    }

    private void Start()
    {

        Vector2[] initialPositions = new Vector2[Tentacle.NumberOfPoints - ThresholdPoint * 2 + 1];

        Tentacle.Point firstOnThreshold = Tentacle.Points[ThresholdPoint];
        Tentacle.Point lastOnThreshold = Tentacle.Points[Tentacle.NumberOfPoints - ThresholdPoint - 1];

        retractingState = new RetractingState(this, initialPositions, firstOnThreshold, lastOnThreshold);
        extendingState = new ExtendingState(this, initialPositions, firstOnThreshold, lastOnThreshold);
        iddleState = new IddleState(this);

        currentState = iddleState;
        currentState.EnterState();

    }

    public void SwitchState (WormStateType type)
    {

        currentState.ExitState();

        switch (type)
        {
            case WormStateType.ExtendingState:
                currentState = extendingState;
                break;

            case WormStateType.RetractingState:
                currentState = retractingState;
                break;
            case WormStateType.IddleState:
                currentState = iddleState;
                break;
        }

        currentState.EnterState();

    }

    void Update()
    {
        currentState.UpdateState();
    }
    private void LateUpdate()
    {
        currentState.LateUpdateState();
    }
}