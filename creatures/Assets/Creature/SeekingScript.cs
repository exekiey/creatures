using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeekingScript : MonoBehaviour
{
    [SerializeField] Vector2 position;

    [SerializeField] PositionTracker positionTracker;

    [SerializeField] float moveForce;

    bool usePathFinding;

    Rigidbody2D _rigidbody2D;

    PathFinding _pathFinding;

    LinkedList<Cell> cellPath;

    Cell nextCell;

    private void Start()
    {

        _rigidbody2D = GetComponent<Rigidbody2D>();

        if (gameObject.TryGetComponent<PathFinding>(out _pathFinding) == true)
        { 

            usePathFinding = true;

            _pathFinding.Destination = position;

            cellPath = _pathFinding.GetPath();

            nextCell = cellPath.First();
            
            cellPath.RemoveFirst();
        }


    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {

        position = positionTracker.Position;

        if (usePathFinding)
        {

            if (cellPath.Count == 0) return;

            if (GridScript.GetCellCoords(transform.position) == nextCell)
            {
                nextCell = cellPath.First();
                cellPath.RemoveFirst();

                position = GridScript.GetRealWorldCoords(nextCell);

                Debug.Log(nextCell.x + ":" + nextCell.y);

            }

        }

        Vector2 direction = (position - (Vector2)transform.position).normalized;

        _rigidbody2D.AddForce(direction * moveForce, ForceMode2D.Force);

    }
}
