using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tentacle))]
public class TentacleLockOnPos : MonoBehaviour
{

    Tentacle tentacle;

    [SerializeField] Vector2 pos;
    [SerializeField] bool doLock;

    // Start is called before the first frame update
    void Awake()
    {
        tentacle = GetComponent<Tentacle>();
    }

    // Update is called once per frame
    void Update()
    {
        if (doLock)
        {
            tentacle.Points[0].locked = true;
            tentacle.Points[0].currentPosition = pos;

        }
    }
}
