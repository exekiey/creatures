using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tentacle))]
public class TentacleCustomRendering : MonoBehaviour
{

    Tentacle tentacle;
    CustomLineRenderer lineRenderer;


    // Start is called before the first frame update
    void Awake()
    {
        tentacle = GetComponent<Tentacle>();  
        lineRenderer = GetComponent<CustomLineRenderer>();
    }

    private void Start()
    {

        lineRenderer.NumberOfPoints = tentacle.NumberOfPoints;
    }

    // Update is called once per frame
    void Update()
    {

        lineRenderer.SetPoints(tentacle.Positions);
    }
}
