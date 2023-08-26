using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTracker : MonoBehaviour
{

    [SerializeField] protected Vector2 position;

    protected PositionSquare positionSquare;

    public Vector2 Position { get => position; }

    // Start is called before the first frame update
    void Start()
    {
        positionSquare = GetComponent<PositionSquare>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        positionSquare.RealPosition = position;
        position = transform.position;
    }
}
