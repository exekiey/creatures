using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;

[RequireComponent(typeof(Tentacle))]
public class TentacleFollowCursor : MonoBehaviour
{

    Tentacle tentacle;
    Camera mainCam;

    [SerializeField] bool first;
    [SerializeField] bool last;

    // Start is called before the first frame update
    void Awake()
    {
        tentacle = GetComponent<Tentacle>();
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        if (first)
        {
            tentacle.Points[0].currentPosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (last)
        {
            tentacle.Points.Last().currentPosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
        }


    }
}
