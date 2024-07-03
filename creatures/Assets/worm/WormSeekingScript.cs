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

    CircleCollider2D moverCollider;


    public RetractingState(WormSeekingScript context) : base(context)
    {

        moverCollider = _context.MoverCircle.GetComponent<CircleCollider2D>();

    }

    public override void EnterState()
    {
        _context.Tentacle.Last.locked = true;

        Vector2 midPoint = (_context.Tentacle.First.currentPosition + _context.Tentacle.Last.currentPosition) / 2;
        direction = _context.Tentacle.Last.currentPosition - _context.Tentacle.First.currentPosition;
        direction = direction.normalized;

        perpendicularDirection = Vector2.Perpendicular(direction);

        _context.MoverCircle.transform.position = midPoint;
        _context.MoverCircle.transform.position -= (Vector3)perpendicularDirection * 0.5f;

        moverCollider = moverCollider.GetComponent<CircleCollider2D>();

        _context.MoverCircle.SetActive(true);
    }

    public override void UpdateState()
    {


        _context.MoverCircle.transform.position += (Vector3) Vector2.up * _context.MoveForce * Time.deltaTime;


        Vector2 midPoint = (_context.Tentacle.First.currentPosition + _context.Tentacle.Last.currentPosition) / 2;

        _context.MoverCircle.transform.position = new Vector3(midPoint.x, _context.MoverCircle.transform.position.y);

        if (Vector2.Distance(_context.Tentacle.First.currentPosition, _context.Tentacle.Last.currentPosition) < _context.SeparationThreshold)
        {

            _context.SwitchState(WormStateType.ExtendingState);

        }

    }

    public override void ExitState()
    {
        _context.MoverCircle.SetActive(false);
    }
}
class ExtendingState : WormState
{
    public ExtendingState (WormSeekingScript context) : base(context)
    {
        _context.Tentacle.Gravity = 0;
    }

    public override void EnterState()
    {
        _context.Tentacle.First.locked = true;
    }
    public override void UpdateState()
    {
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

    [SerializeField] GameObject moverCircle;

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
    public GameObject MoverCircle { get => moverCircle; set => moverCircle = value; }

    private void Awake()
    {
        tentacle = GetComponent<Tentacle>();
    }

    private void Start()
    {

        Vector2[] initialPositions = new Vector2[Tentacle.NumberOfPoints - ThresholdPoint * 2 + 1];

        Tentacle.Point firstOnThreshold = Tentacle.Points[ThresholdPoint];
        Tentacle.Point lastOnThreshold = Tentacle.Points[Tentacle.NumberOfPoints - ThresholdPoint - 1];

        retractingState = new RetractingState(this);
        extendingState = new ExtendingState(this);
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