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

    bool usePathFinding;

    Rigidbody2D _rigidbody2D;

    PathFinding _pathFinding;

    LinkedList<Cell> cellPath;

    Cell nextCell;

    Camera mainCam;

    private void Start()
    {

        _rigidbody2D = GetComponent<Rigidbody2D>();

        targetPosition = tarjetObject.transform.position;

        if (gameObject.TryGetComponent<PathFinding>(out _pathFinding) == true)
        { 

            usePathFinding = true;

            _pathFinding.Destination = targetPosition;

            cellPath = _pathFinding.GetPath();

            nextCell = cellPath.First();
            
            cellPath.RemoveFirst();
        }

        mainCam = Camera.main;

    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        /*
        if (followCursor)
        {
            position = mainCam.ScreenToWorldPoint(position);
        }
        else
        {
            position = tarjetObject.transform.position;
            Debug.Log(position);
        }*/
        
        if (usePathFinding)
        {

            if (cellPath.Count > 0)
            {
                if (GridScript.GetCellCoords(transform.position) == nextCell)
                {
                    nextCell = cellPath.First();
                    cellPath.RemoveFirst();

                    targetPosition = GridScript.GetRealWorldCoords(nextCell);

                }
            }



        }
        
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        _rigidbody2D.AddForce(direction * moveForce, ForceMode2D.Force);

    }
}
