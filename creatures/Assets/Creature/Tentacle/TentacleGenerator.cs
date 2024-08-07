using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;




<<<<<<< Updated upstream:creatures/Assets/Creature/Tentacle/TentacleGenerator.cs


public class TentacleGenerator : MonoBehaviour
=======
public class Tentacle : MonoBehaviour
>>>>>>> Stashed changes:creatures/Assets/Creature/Tentacle/Tentacle.cs
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

    [SerializeField] int numberOfPoints;
    [SerializeField] float tentacleLength;
    [SerializeField] int distanceCheckIterations = 50;
    [SerializeField] float gravity = 10f;
<<<<<<< Updated upstream:creatures/Assets/Creature/Tentacle/TentacleGenerator.cs
    [SerializeField] private bool followCursor;
    [SerializeField] float tentacleWidth;
=======
    [SerializeField] float loseness;
    [SerializeField] float generationDistance;
    [SerializeField] string[] collidingLayers;

    [SerializeField] bool _isColliding;
>>>>>>> Stashed changes:creatures/Assets/Creature/Tentacle/Tentacle.cs

    LineRenderer lineRenderer;

    float segmentLength;
    private float halfSegment;
    Vector2 origin;
    Point[] points;
    Vector2[] positions;
    Point last;

    [SerializeField] SuckerScript sucker;


    //collision
    const int maxCollisions = 32;
    const float collisionRadius = .5f;
    const int colliderBufferSize = 8;

    int numCollisions;
    CollisionInfo[] collisionInfos;
    Collider2D[] colliderBuffer;
    bool shouldSnapShotCollision;
<<<<<<< Updated upstream:creatures/Assets/Creature/Tentacle/TentacleGenerator.cs
=======
    private float separation;

    public Point[] Points { get => points; set => points = value; }
    public Vector2[] Positions { get =>  positions; }

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
>>>>>>> Stashed changes:creatures/Assets/Creature/Tentacle/Tentacle.cs

    // Start is called before the first frame update
    void Awake()
    {

<<<<<<< Updated upstream:creatures/Assets/Creature/Tentacle/TentacleGenerator.cs
=======
        
>>>>>>> Stashed changes:creatures/Assets/Creature/Tentacle/Tentacle.cs
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numberOfPoints;
        

        segmentLength = tentacleLength / (numberOfPoints - 1);

        points = new Point[numberOfPoints];
        positions = new Vector2[numberOfPoints];

        origin = transform.position;

        GenerateTentacle();

        GenerateCollisionInfo();

        points.First().locked = false;

        last = points.Last();
<<<<<<< Updated upstream:creatures/Assets/Creature/Tentacle/TentacleGenerator.cs
=======
        first = points.First();
        midPoint = points[numberOfPoints / 2];
>>>>>>> Stashed changes:creatures/Assets/Creature/Tentacle/Tentacle.cs


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

<<<<<<< Updated upstream:creatures/Assets/Creature/Tentacle/TentacleGenerator.cs
            int collisions = Physics2D.OverlapCircleNonAlloc(currentPoint.currentPosition, collisionRadius, colliderBuffer);
=======
            int collisions = Physics2D.OverlapCircleNonAlloc(currentPoint.currentPosition, collisionRadius, colliderBuffer, layerMask: LayerMask.GetMask(collidingLayers));
>>>>>>> Stashed changes:creatures/Assets/Creature/Tentacle/Tentacle.cs

            Debug.Log(currentPoint.id + ":" + collisions);

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

                    //Debug.Log(currentCollisionInfo.PrintFirstId());

                    Debug.Log("from snapshot: " + currentCollisionInfo.id + "id: " + currentCollisionInfo.collidingPoints[0].id);

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


        Debug.Log("......");
    }


    void AdjustCollisions()
    {


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

                        Vector2 collisionDirection = (currentPoint.currentPosition - currentCollisionInfo.pos).normalized;
                        Vector2 hitPos = currentCollisionInfo.pos + collisionDirection * radius;

                        currentPoint.currentPosition = hitPos;
                    }

                    break;

                case ColliderType.Box:

                    for (int j = 0; j < currentCollisionInfo.numberOfCollidingPoints; j++)
                    {

                        Point currentPoint = currentCollisionInfo.collidingPoints[j];

                        Vector2 pointInLocal = currentCollisionInfo.localToWorld.MultiplyPoint(currentPoint.currentPosition);

                        Vector2 halfSize = currentCollisionInfo.size / 2;
                        Vector2 scalar = currentCollisionInfo.scale;

                        float absouluteCollisionPointX = halfSize.x - Mathf.Abs(pointInLocal.x);

                        if (absouluteCollisionPointX <= 0)
                        {
                            continue;
                        }

                        float absouluteCollisionPointY = halfSize.y - Mathf.Abs(pointInLocal.y);

                        if (absouluteCollisionPointX <= 0)
                        {
                            continue;
                        }

                        if (absouluteCollisionPointX * scalar.x < absouluteCollisionPointY * scalar.x)
                        {

                            float collisionDirection = Mathf.Sign(pointInLocal.x);
                            pointInLocal.x = halfSize.x * collisionDirection;
                        }
                        else
                        {
                            float collisionDirection = Mathf.Sign(pointInLocal.y);
                            pointInLocal.x = halfSize.x * collisionDirection;
                        }

                        Vector2 hitPos = currentCollisionInfo.localToWorld.MultiplyPoint(pointInLocal);

                        currentPoint.currentPosition = hitPos;


                    }
                    break;
            }

        }

    }

    void GenerateTentacle()
    {
        float firstCenter = origin.x;

        Point firstLeftPoint = new Point(new Vector2(firstCenter, origin.y), new Vector2(firstCenter, origin.y));

        points[0] = firstLeftPoint;

        for (int i = 1; i < numberOfPoints; i++)
        {


            float center = segmentLength * i + origin.x;

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

    void DistanceConstraint()
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

<<<<<<< Updated upstream:creatures/Assets/Creature/Tentacle/TentacleGenerator.cs

=======
            positions[i] = currentPos;

            if (isLast)
            {

                currentTotalLength += separation;
            }
>>>>>>> Stashed changes:creatures/Assets/Creature/Tentacle/Tentacle.cs
        }


        positions[numberOfPoints - 1] = points[numberOfPoints - 1].currentPosition;

        //Debug.Log("----------");
    }

    private void FixedUpdate()
    {
        shouldSnapShotCollision = true;
    }

    private void FirstAndLastDistanceConstraint()
    {


        Point first = points.First();

        if (first.locked && last.locked) return;

        float separation = Vector2.Distance(first.currentPosition, last.currentPosition);

        if (separation < tentacleLength) return;

        float error = separation - tentacleLength;

        Vector2 orientation = (first.currentPosition - last.currentPosition).normalized;

        Vector2 correctionVector = orientation * error;


        if (first.locked && separation > tentacleLength)
        {
            last.currentPosition += correctionVector;
            return;
        }

        if (last.locked && separation > tentacleLength)
        {
            first.currentPosition -= correctionVector;
            return;
        }

        last.currentPosition += correctionVector / 2;
        first.currentPosition -= correctionVector / 2;


    }

    void AttachToBody()
    {



    }

    void SimulateTentacle()
    {

        if (shouldSnapShotCollision)
        {
            SnapshotCollision();
        }

        AttachToBody();

        MoveSimulation();
        //FirstAndLastDistanceConstraint();
        AdjustCollisions();
        for (int i = 0; i < distanceCheckIterations; i++)
        {
            DistanceConstraint();
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
<<<<<<< Updated upstream:creatures/Assets/Creature/Tentacle/TentacleGenerator.cs

=======
        /*
>>>>>>> Stashed changes:creatures/Assets/Creature/Tentacle/Tentacle.cs
        if (points != null)
        {

            foreach (Point point in points)
            {

                Handles.Label(point.currentPosition, point.id.ToString());

                if (point.locked)
                {
                    Gizmos.DrawIcon(point.currentPosition, "sv_icon_dot6_pix16_gizmo");
                    continue;
                }

                if (point == last)
                {
                    Gizmos.DrawIcon(point.currentPosition, "sv_icon_dot3_pix16_gizmo");
                }
                else
                {
                    Gizmos.DrawIcon(point.currentPosition, "sv_icon_dot1_pix16_gizmo");
                }

                Gizmos.color = new Color(0, 1, 0, 0.5f);

                Gizmos.DrawSphere(point.currentPosition, collisionRadius);
            }
<<<<<<< Updated upstream:creatures/Assets/Creature/Tentacle/TentacleGenerator.cs


        }
=======
            
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;

            Handles.Label(points.Last().currentPosition + Vector2.left * 0.5f, currentTotalLength.ToString(), style);
            


        }
        */
        

#if (UNITY_EDITOR)
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * generationDistance);
#endif
>>>>>>> Stashed changes:creatures/Assets/Creature/Tentacle/Tentacle.cs

    }
}