using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TentaclePathFindingForceContributor : MonoBehaviour
{

    [SerializeField] SeekingScript body;
    Rigidbody2D parentRigidbody2D;
    float forceContribution;
    float gravityContribution;


    // Start is called before the first frame update
    void Start()
    {

        parentRigidbody2D = GetComponentInParent<Rigidbody2D>();

        int numberOfActiveTentacleChildren = GetComponentsInChildren<TentaclePathFinding>().Count();

        forceContribution = body.MoveForce / numberOfActiveTentacleChildren;
        gravityContribution = parentRigidbody2D.gravityScale / numberOfActiveTentacleChildren;

        for (int i = 0; i < numberOfActiveTentacleChildren; i++)
        {

            ContributeForceNegative();

        }

    }

    public void ContributeForcePositive()
    {

        body.MoveForce += forceContribution;

    }
    public void ContributeForceNegative()
    {

        body.MoveForce -= forceContribution;

    }

    public void ContributeGravityPositive()
    {

        parentRigidbody2D.gravityScale += gravityContribution;

    }
    public void ContributeGravityNegative()
    {

        parentRigidbody2D.gravityScale -= gravityContribution;

    }

}
