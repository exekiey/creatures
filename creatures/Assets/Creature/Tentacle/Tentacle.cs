using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;






[RequireComponent(typeof(LineRenderer))]
public class Tentacle : MonoBehaviour
{

    enum ColliderType
    {

        Circle,
        Box,
        None,

    }

    public class Point
    {

        static int idCounter;
        public int id;

        public Vector2 currentPosition;
        public Vector2 previousPosition;
        public bool locked;

        public Point(Vector2 currentPosition, Vector2 previousPosition, bool locked)
        {
            this.currentPosition = currentPosition;
            this.previousPosition = previousPosition;
            this.locked = locked;

        }

        public Point(Vector2 currentPosition, Vector2 previousPosition)
        {

            this.currentPosition = currentPosition;
            this.previousPosition = previousPosition;
            id = idCounter++;

        }

    }
    class CollisionInfo
    {

        public int id;
        public ColliderType type;
        public Vector2 size;
        public Vector2 pos;
        public Vector2 scale;
        public Matrix4x4 worldToLocal;
        public Matrix4x4 localToWorld;
        public int numberOfCollidingPoints;
        public Point[] collidingPoints;

        public CollisionInfo(int maxCollisions)
        {

            id = -1;
            type = ColliderType.None;
            size = Vector2.zero;
            pos = Vector2.zero;
            scale = Vector2.zero;
            worldToLocal = Matrix4x4.zero;
            localToWorld = Matrix4x4.zero;

            numberOfCollidingPoints = 0;
            collidingPoints = new Point[maxCollisions];

        }

        public int PrintFirstId()
        {

            return (collidingPoints[0].id);
        }

    }

    //data
    [SerializeField] int numberOfPoints;
    [SerializeField] float tentacleLength;
    [SerializeField] int distanceCheckIterations = 50;
    [SerializeField] int rotationChecktIterations;
    [SerializeField] float gravity = 10f;
    [SerializeField] float loseness;
    [SerializeField] float generationDistance;
    [SerializeField] float minAngle;

    [SerializeField] bool _isColliding;

    LineRenderer lineRenderer;

    float segmentLength;
    Vector2 origin;
    Point[] points;
    Point last;
    Point first;
    Point midPoint;
    [SerializeField] float currentTotalLength;


    //collision
    const int maxCollisions = 32;
    const float collisionRadius = .5f;
    const int colliderBufferSize = 8;

    int numCollisions;
    CollisionInfo[] collisionInfos;
    Collider2D[] colliderBuffer;
    bool shouldSnapShotCollision;
    private float separation;

    LayerMask collisionLayerMask;

    public Point[] Points { get => points; set => points = value; }
    public float CurrentTotalLength { get => currentTotalLength; }
    public float TentacleLength { get => tentacleLength; }
    public float Loseness { get => loseness; set => loseness = value; }
    public bool IsColliding { get => _isColliding; }
    public float Separation { get => separation; }
    public Point Last { get => last; }
    public Point First { get => first; }
    public Point MidPoint { get => midPoint; }
    public int NumberOfPoints { get => numberOfPoints; set => numberOfPoints = value; }
    public float Gravity { get => gravity; set => gravity = value; }

    float angleCorrection;

    // Start is called before the first frame update
    void Awake()
    {

        collisionLayerMask = LayerMask.GetMask("obstacle");

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numberOfPoints;

        segmentLength = tentacleLength / (numberOfPoints - 1);

        points = new Point[numberOfPoints];

        origin = transform.position;

        GenerateTentacle();

        GenerateCollisionInfo();


        last = points.Last();
        first = points.First();
        midPoint = points[numberOfPoints / 2];
        angleCorrection = minAngle * Mathf.Deg2Rad / 2;


    }

    void GenerateCollisionInfo()
    {

        collisionInfos = new CollisionInfo[maxCollisions];

        for (int i = 0; i < collisionInfos.Length; i++)
        {

            collisionInfos[i] = new CollisionInfo(numberOfPoints);
        }

        colliderBuffer = new Collider2D[colliderBufferSize];

    }

    void SnapshotCollision()
    {

        numCollisions = 0;

        foreach (Point currentPoint in points)
        {

            int collisions = Physics2D.OverlapCircleNonAlloc(currentPoint.currentPosition, collisionRadius, colliderBuffer, layerMask: collisionLayerMask);


            //loop through every collider that the current point is colliding with
            for (int i = 0; i < collisions; i++)
            {

                Collider2D currentCollider = colliderBuffer[i];


                int id = currentCollider.GetInstanceID();

                int idx = -1;

                //loop through every collision in the frame to check if the current collider has already been stored
                for (int k = 0; k < numCollisions; k++)
                {

                    if (collisionInfos[k].id == id)
                    {
                        //if it was stored, don't store it again
                        idx = k;
                        break;

                    }

                }

                bool isTheColliderNew = idx < 0;
                if (isTheColliderNew)
                {

                    CollisionInfo currentCollisionInfo = collisionInfos[numCollisions];

                    currentCollisionInfo.id = id;

                    currentCollisionInfo.worldToLocal = currentCollider.transform.worldToLocalMatrix;
                    currentCollisionInfo.localToWorld = currentCollider.transform.localToWorldMatrix;

                    currentCollisionInfo.scale.x = currentCollisionInfo.localToWorld.GetColumn(0).magnitude;
                    currentCollisionInfo.scale.y = currentCollisionInfo.localToWorld.GetColumn(1).magnitude;

                    currentCollisionInfo.pos = currentCollider.transform.position;

                    currentCollisionInfo.numberOfCollidingPoints = 1;
                    currentCollisionInfo.collidingPoints[0] = currentPoint;

                    switch (currentCollider)
                    {
                        case CircleCollider2D circle:
                            currentCollisionInfo.type = ColliderType.Circle;
                            currentCollisionInfo.size.x = currentCollisionInfo.size.y = circle.radius;
                            break;
                        case BoxCollider2D box:
                            currentCollisionInfo.type = ColliderType.Box;
                            currentCollisionInfo.size = box.size;
                            break;
                        default:
                            currentCollisionInfo.type = ColliderType.None;
                            break;
                    }

                    numCollisions++;

                    if (numCollisions >= maxCollisions)
                    {
                        return;
                    }

                }
                else
                {
                    CollisionInfo currentCollisionInfo = collisionInfos[idx];

                    if (currentCollisionInfo.numberOfCollidingPoints >= numberOfPoints)
                    {

                        continue;

                    }

                    currentCollisionInfo.collidingPoints[currentCollisionInfo.numberOfCollidingPoints++] = currentPoint;

                }

            }


        }

        shouldSnapShotCollision = false;
    }


    void AdjustCollisions()
    {

        bool isAnyColliding = false;

        for (int i = 0; i < numCollisions; i++)
        {

            CollisionInfo currentCollisionInfo = collisionInfos[i];

            switch (currentCollisionInfo.type)
            {

                case ColliderType.Circle:

                    float radius = currentCollisionInfo.size.x * Mathf.Max(currentCollisionInfo.scale.x, currentCollisionInfo.scale.y);

                    for (int j = 0; j < currentCollisionInfo.numberOfCollidingPoints; j++)
                    {
                        Point currentPoint = currentCollisionInfo.collidingPoints[j];

                        float distance = Vector2.Distance(currentCollisionInfo.pos, currentPoint.currentPosition);

                        // Debug.Log("from adjust: " + currentCollisionInfo.id + "id: " + currentCollisionInfo.collidingPoints[0].id + "distance:" + distance);

                        bool isColliding = distance < radius;

                        if (!isColliding)
                        {
                            continue;
                        }

                        isAnyColliding = true;

                        Vector2 collisionDirection = (currentPoint.currentPosition - currentCollisionInfo.pos).normalized;
                        Vector2 hitPos = currentCollisionInfo.pos + collisionDirection * radius;

                        currentPoint.currentPosition = hitPos;
                    }

                    break;

                case ColliderType.Box:

                    for (int j = 0; j < currentCollisionInfo.numberOfCollidingPoints; j++)
                    {

                        Point currentPoint = currentCollisionInfo.collidingPoints[j];

                        Vector2 pointInLocal = currentCollisionInfo.worldToLocal.MultiplyPoint(currentPoint.currentPosition);

                        Vector2 halfSize = currentCollisionInfo.size / 2;
                        Vector2 scalar = currentCollisionInfo.scale;

                        float absouluteCollisionPointX = halfSize.x - Mathf.Abs(pointInLocal.x);

                        if (absouluteCollisionPointX <= 0)
                        {
                            continue;
                        }

                        float absouluteCollisionPointY = halfSize.y - Mathf.Abs(pointInLocal.y);

                        if (absouluteCollisionPointY <= 0)
                        {
                            continue;
                        }

                        isAnyColliding = true;

                        if (absouluteCollisionPointX * scalar.x < absouluteCollisionPointY * scalar.y)
                        {

                            float collisionDirection = Mathf.Sign(pointInLocal.x);
                            pointInLocal.x = halfSize.x * collisionDirection;
                        }
                        else
                        {
                            float collisionDirection = Mathf.Sign(pointInLocal.y);
                            pointInLocal.y = halfSize.y * collisionDirection;
                        }

                        Vector2 hitPos = currentCollisionInfo.localToWorld.MultiplyPoint(pointInLocal);

                        currentPoint.currentPosition = hitPos;


                    }
                    break;
            }

        }

        _isColliding = isAnyColliding;

    }

    void GenerateTentacle()
    {

        float startingSegmentLength = generationDistance / numberOfPoints;

        float firstCenter = origin.x;

        Point firstLeftPoint = new Point(new Vector2(firstCenter, origin.y), new Vector2(firstCenter, origin.y));

        points[0] = firstLeftPoint;

        for (int i = 1; i < numberOfPoints; i++)
        {


            float center = startingSegmentLength * i + origin.x;

            Point rightPoint = new Point(new Vector2(center, origin.y), new Vector2(center, origin.y));

            points[i] = rightPoint;

        }


    }

    private void MoveSimulation()
    {


        foreach (Point currentPoint in points)
        {

            Vector2 positionBeforeUpdate = currentPoint.currentPosition;
            if (!currentPoint.locked)
            {


                Vector2 currentVelocity = currentPoint.currentPosition - currentPoint.previousPosition;

                currentPoint.currentPosition += currentVelocity;

                //currentPoint.currentPosition += Vector2.down * gravity * Time.deltaTime;
                currentPoint.currentPosition += Vector2.down * gravity * Time.deltaTime * Time.deltaTime;
            }


            currentPoint.previousPosition = positionBeforeUpdate;
        }
    }

    void DistanceConstraint(bool isLast)
    {

        for (int i = 0; i < numberOfPoints - 1; i++)
        {
            Vector2 currentPos = points[i].currentPosition;
            Vector2 nextPos = points[i + 1].currentPosition;


            Vector2 segmentOrientation = (nextPos - currentPos).normalized;

            float separation = Vector2.Distance(currentPos, nextPos);
            float error = separation - segmentLength;


            //Debug.Log($"CurrentPos: {currentPos}, NextPos: {nextPos}, SegmentOrientation: {segmentOrientation}, Separation: {separation}, Error: {error}, FixedCurrent: {fixedCurrent}, FixedNext: {fixedNext}");
            //Debug.Log(currentNode.Value.id + ":" + separation);

            if (!points[i].locked)
            {
                Vector2 fixedCurrent = currentPos + segmentOrientation * (error * 0.5f);
                points[i].currentPosition = fixedCurrent;
            }

            if (!points[i + 1].locked)
            {
                Vector2 fixedNext = nextPos - segmentOrientation * (error * 0.5f);
                points[i + 1].currentPosition = fixedNext;
            }

            if (isLast)
            {

                currentTotalLength += separation;
            }
            //RotatePoints(i);
        }

        //Debug.Log("----------");
    }

    void RotationConstraint()
    {

        for (int i = 1; i <  numberOfPoints - 1; i++)
        {
            RotatePoints(i);
        }


    }

    private void RotatePoints(int i)
    {

        if (i == 0 || i == numberOfPoints - 1) return;

        Point currentPoint = points[i];
        Point previousPoint = points[i - 1];
        Point nextPoint = points[i + 1];

        Vector2 previousVector = previousPoint.currentPosition - currentPoint.currentPosition;
        Vector2 nextVector = nextPoint.currentPosition - currentPoint.currentPosition;

        float angle = Vector2.Angle(previousVector, nextVector);

        if (angle < minAngle)
        {

            //

            Vector2 bisection = (previousVector + nextVector) / 2;

            if (!previousPoint.locked)
            {

                FixPointRotationLeft(currentPoint, previousPoint, previousVector, bisection);
            }

            if (!nextPoint.locked)
            {
                FixPointRotationRight(currentPoint, nextPoint, nextVector, bisection);
            }
        }
    }

    private void FixPointRotationRight(Tentacle.Point currentPoint, Tentacle.Point point, Vector2 vector, Vector2 bisection)
    {


        Vector2 orientation = Vector2.Perpendicular(bisection).normalized;
        Debug.DrawLine(currentPoint.currentPosition, currentPoint.currentPosition + orientation);
        Debug.DrawLine(currentPoint.currentPosition, currentPoint.currentPosition - orientation);

        float orientationAngle = Mathf.Atan2(bisection.y, bisection.x);

        orientationAngle -= angleCorrection;

        float length = vector.magnitude;


        float correctionX = length * Mathf.Cos(orientationAngle);
        float correctionY = length * Mathf.Sin(orientationAngle);

        Vector2 correctionVector = new Vector2(correctionX, correctionY);

        Vector2 fixedPosition = currentPoint.currentPosition + correctionVector;

        Debug.DrawLine(currentPoint.currentPosition, fixedPosition, Color.red);

        point.currentPosition = fixedPosition;

    }

    private void FixPointRotationLeft(Tentacle.Point currentPoint, Tentacle.Point point, Vector2 vector, Vector2 bisection)
    {


        Vector2 orientation = Vector2.Perpendicular(bisection).normalized;
        Debug.DrawLine(currentPoint.currentPosition, currentPoint.currentPosition + orientation);
        Debug.DrawLine(currentPoint.currentPosition, currentPoint.currentPosition - orientation);

        float orientationAngle = Mathf.Atan2(bisection.y, bisection.x);

        orientationAngle += angleCorrection;

        float length = vector.magnitude;


        float correctionX = length * Mathf.Cos(orientationAngle);
        float correctionY = length * Mathf.Sin(orientationAngle);

        Vector2 correctionVector = new Vector2(correctionX, correctionY);

        Vector2 fixedPosition = currentPoint.currentPosition + correctionVector;

        point.currentPosition = fixedPosition;

    }
    private void FixedUpdate()
    {
        shouldSnapShotCollision = true;
    }

    private void FirstAndLastDistanceConstraint()
    {


        Point first = points.First();

        separation = Vector2.Distance(first.currentPosition, last.currentPosition);

        if (first.locked && last.locked) return;

        if (separation < tentacleLength) return;

        float error = separation - tentacleLength;

        Vector2 orientation = (first.currentPosition - last.currentPosition).normalized;

        Vector2 correctionVector = orientation * error;



        if (first.locked && separation + loseness > tentacleLength)
        {
            last.currentPosition += correctionVector;
            return;
        }

        if (last.locked && separation + loseness > tentacleLength)
        {
            first.currentPosition -= correctionVector;
            return;
        }

        last.currentPosition += correctionVector / 2;
        first.currentPosition -= correctionVector / 2;


    }

    void SimulateTentacle()
    {

        if (shouldSnapShotCollision)
        {
            SnapshotCollision();
        }

        MoveSimulation();
        FirstAndLastDistanceConstraint();

        currentTotalLength = 0;


        for (int _ = 0; _ < distanceCheckIterations; _++)
        {
            AdjustCollisions();
            DistanceConstraint(_ == distanceCheckIterations - 1);
        }




    }



    void DrawLine()
    {

        Vector3[] linePoints = new Vector3[numberOfPoints];

        int counter = 0;

        foreach (Point currentPoint in points)
        {

            linePoints[counter] = currentPoint.currentPosition;
            counter++;
        }

        lineRenderer.SetPositions(linePoints);

    }

    // Update is called once per frame
    void Update()
    {
        SimulateTentacle();

        DrawLine();

    }



    private void OnDrawGizmos()
    {
        
        if (points != null)
        {

            foreach (Point point in points)
            {

                //Handles.Label(point.currentPosition, point.id.ToString());

                if (point.locked)
                {
                    Gizmos.DrawIcon(point.currentPosition, "sv_icon_dot6_pix16_gizmo");
                    continue;
                }

                if (point == points.Last())
                {
                    Gizmos.DrawIcon(point.currentPosition, "sv_icon_dot3_pix16_gizmo");
                }
                else
                {
                    Gizmos.DrawIcon(point.currentPosition, "sv_icon_dot1_pix16_gizmo");
                }

                Gizmos.color = new Color(0, 1, 0, 0.5f);

                //Gizmos.DrawSphere(point.currentPosition, collisionRadius);
            }
            /*
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;

            Handles.Label(points.Last().currentPosition + Vector2.left * 0.5f, currentTotalLength.ToString(), style);
            */


        }

        

#if (UNITY_EDITOR)
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * generationDistance);
#endif

    }
}