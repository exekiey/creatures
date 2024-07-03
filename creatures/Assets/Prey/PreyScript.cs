using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyScript : MonoBehaviour
{

    public event Action changedCell;

    Cell previousCell;

    [SerializeField]Vector2 currenCel;

    private void Start()
    {
        previousCell = GridScript.GetCellCoords(transform.position);
    }

    void Update()
    {
        currenCel = new Vector2(previousCell.x, previousCell.y);



        Cell currentCell = GridScript.GetCellCoords(transform.position);



        if (currentCell != previousCell)
        {
            changedCell?.Invoke();
        }

        previousCell = currentCell;

    }

}
