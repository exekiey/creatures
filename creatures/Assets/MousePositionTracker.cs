using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePositionTracker : PositionTracker
{


    // Update is called once per frame
    /*
    private void Update()
    {
        position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    }*/

    override protected void Update()
    {
        base.Update();
        position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    }



}
