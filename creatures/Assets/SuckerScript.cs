using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuckerScript : MonoBehaviour
{

    [SerializeField] HingeJoint2D tentacleJoint;
    [SerializeField] HingeJoint2D suckerJoint;
    DistanceJoint2D distanceJoint2D;


    private void Awake()
    {
        distanceJoint2D = GetComponent<DistanceJoint2D>();
    }

    public HingeJoint2D TentacleJoint { get => tentacleJoint;}
    public HingeJoint2D SuckerJoint { get => suckerJoint;}
    public GameObject Object { get => gameObject; }
    public float Distance { set => distanceJoint2D.distance = value; }
}
