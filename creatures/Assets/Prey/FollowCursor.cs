using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{

    Camera main;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Awake()
    {
        main = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newPosition = main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction = newPosition - (Vector2) transform.position;

        
        if (Physics2D.OverlapPoint(newPosition, LayerMask.GetMask("obstacle")))
        {

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, float.PositiveInfinity, LayerMask.GetMask("obstacle"));
            Vector2 hitPos = hit.point;

            Vector2 oppositeDirection = direction * -1;

            float aBit = 0.1f;

            Vector2 aBitFurther = oppositeDirection.normalized * aBit;

            transform.position = hitPos + aBitFurther;

        } else
        {
            transform.position = newPosition;
        }





    }
}
