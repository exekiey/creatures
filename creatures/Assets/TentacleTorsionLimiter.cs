using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Tentacle))]
public class TentacleTorsionLimiter : MonoBehaviour
{
    Tentacle tentacle;

    [SerializeField] float minAngle;
    [SerializeField] int rotationCheckIterations;

    float cachedCorrectionAngle;

    // Start is called before the first frame update

    private void Awake()
    {
        tentacle = GetComponent<Tentacle>();
        cachedCorrectionAngle = minAngle * Mathf.Deg2Rad / 2;
    }

    void Start()
    {

        //tentacle.Points[1].locked = true;   

    }

    // Update is called once per frame
    void LateUpdate()
    {
        

        for (int _ = 0; _ < rotationCheckIterations; _++)
        {

            RotationConstraint();


        }

 
        
    }

    void RotationConstraint()
    {

        for (int i = 1; i < tentacle.NumberOfPoints - 1; i++)
        {
            RotatePoints(i);
        }


    }

    private void RotatePoints(int i)
    {

        if (i == 0 || i == tentacle.NumberOfPoints - 1) return;

        Tentacle.Point currentPoint = tentacle.Points[i];
        Tentacle.Point previousPoint = tentacle.Points[i - 1];
        Tentacle.Point nextPoint = tentacle.Points[i + 1];

        Vector2 previousVector = previousPoint.currentPosition - currentPoint.currentPosition;
        Vector2 nextVector = nextPoint.currentPosition - currentPoint.currentPosition;

        float angle = Vector2.SignedAngle(previousVector, nextVector);

        if (Mathf.Abs(angle) < minAngle)
        {

            //

            Vector2 bisection = (previousVector + nextVector) / 2;

            bisection = bisection.normalized;

            if (!previousPoint.locked)
            {
                FixRotation(currentPoint, previousPoint, previousVector, angle, bisection, -1);
            }

            if (!nextPoint.locked)
            {
                FixRotation(currentPoint, nextPoint, nextVector, angle, bisection, 1);
            }
        }
    }


    Vector2 RotateVector(Vector2 vector, float rotationDegree)
    {

        float length = vector.magnitude;

        float originalRotation = Mathf.Atan2(vector.y, vector.x);

        float newRotation = originalRotation + rotationDegree;

        float correctionX = Mathf.Cos(newRotation) * length;
        float correctionY = Mathf.Sin(newRotation) * length;

        Vector2 correctionVector = new Vector2(correctionX, correctionY);

        return correctionVector;


    }

    private void FixRotation(Tentacle.Point currentPoint, Tentacle.Point point, Vector2 vector, float angle, Vector2 bisection, int positive)
    {

        Vector2 rotatedBisection;

        if (angle < 0)
        {
            rotatedBisection = RotateVector(bisection, -cachedCorrectionAngle * Mathf.Sign(positive));
        } else
        {
            rotatedBisection = RotateVector(bisection, +cachedCorrectionAngle * Mathf.Sign(positive));
        }

        rotatedBisection *= vector.magnitude;

        Vector2 fixedPos = currentPoint.currentPosition + rotatedBisection;



        Debug.DrawLine(currentPoint.currentPosition, fixedPos, Color.red);

        point.currentPosition = fixedPos;

    }

}
