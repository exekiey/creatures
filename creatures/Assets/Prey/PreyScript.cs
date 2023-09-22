using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyScript : MonoBehaviour
{

    public event Action changedCell;

    Cell currentCell;

    private void Start()
    {
        currentCell = GridScript.GetCellCoords(transform.position);
    }

    void Update()
    {

        if (GridScript.GetCellCoords(transform.position) != currentCell)
        {
            changedCell?.Invoke();
        }

    }

}
