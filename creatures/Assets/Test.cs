using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
<<<<<<< Updated upstream

    Rigidbody2D rigidbody2D;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigidbody2D.AddForce(new Vector2(1, 0));
    }
=======
    Tentacle tentacle;

    [SerializeField] Vector2 pos;

    [SerializeField] Rigidbody2D rb;

    private void Start()
    {

        rb.MovePosition(pos);

    }



    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(pos, 0.1f);
    }
>>>>>>> Stashed changes
}
