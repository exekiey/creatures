using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Test : MonoBehaviour
{
    Tentacle tentacle;
    [SerializeField] float force;

    private void Awake()
    {
        tentacle = GetComponent<Tentacle>();
    }

    private void Update()
    {


        tentacle.MidPoint.currentPosition += Vector2.up * force * Time.deltaTime;

    }

}
