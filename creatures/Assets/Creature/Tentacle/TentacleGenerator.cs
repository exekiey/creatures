using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

enum ColliderType
{

    Circle,
    Box,
    None,

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
    public int numCollisions;
    public int[] collidingNodes;

    public CollisionInfo(int maxCollisions)
    {

        id = -1;
        type = ColliderType.None;
        size = Vector2.zero;
        pos = Vector2.zero;
        scale = Vector2.zero;
        worldToLocal = Matrix4x4.zero;
        localToWorld = Matrix4x4.zero;

        numCollisions = 0;
        collidingNodes = new int[maxCollisions];

    } 

}


public class TentacleGenerator : MonoBehaviour
{
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


    [SerializeField] int numberOfPoints;
    [SerializeField] float tentacleLength;
    [SerializeField] int distanceCheckIterations = 50;
    [SerializeField] float gravity = 10f;
    [SerializeField] private bool followCursor;
    [SerializeField] float tentacleWidth;
    
    LineRenderer lineRenderer;

    float segmentLength;
    private float halfSegment;
    Vector2 origin;
    LinkedList<Point> points;
    Point last;

    [SerializeField] SuckerScript sucker;


    //collision
    const int maxCollisions = 32;
    const float collisionRadius = .5f;
    const int colliderBufferSize = 8;

    int numCollisions;
    CollisionInfo[] collisionsInfo;
    Collider2D[] colliderBuffer;

    // Start is called before the first frame update
    void Awake()
    {

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numberOfPoints;

        segmentLength = tentacleLength / (numberOfPoints + 1);

        points = new LinkedList<Point>();

        origin = transform.position;

        GenerateTentacle();

        GenerateCollisionInfo();

        points.First().locked = false;

        last = points.Last();


    }

    void GenerateCollisionInfo()
    {

        collisionsInfo = new CollisionInfo[maxCollisions];

        for (int i = 0; i < collisionsInfo.Length; i++)
        {

            collisionsInfo = new CollisionInfo[numberOfPoints];

        }

        colliderBuffer = new Collider2D[colliderBufferSize];

    }

    void SnapshotCollision()
    {

        numCollisions = 0;

        foreach (Point currentPoint in points)
        {

            int collisions = Physics2D.OverlapCircleNonAlloc(currentPoint.currentPosition, collisionRadius, colliderBuffer);

            //loop through every collider that the current point is colliding with
            for (int i = 0; i < collisions;i++)
            {

                Collider2D currentCollider = colliderBuffer[i];

                int id = currentCollider.GetInstanceID();

                int idx = -1;

                //loop through every collision in the frame to check if the current collider has already been stored
                for (int k  = 0; k < numCollisions; k++)
                {

                    if (collisionsInfo[k].id == id)
                    {
                        //if it was stored, don't store it again
                        idx = k;
                        break;

                    }

                }


                if (idx > 0)
                {

                    CollisionInfo currentCollisionInfo = collisionsInfo[numCollisions];
                    currentCollisionInfo.id = id;
                    
                    currentCollisionInfo.worldToLocal = currentCollider.transform.worldToLocalMatrix;
                    currentCollisionInfo.localToWorld = currentCollider.transform.localToWorldMatrix;

                    currentCollisionInfo.scale.x = currentCollisionInfo.localToWorld.GetColumn(0).magnitude;
                    currentCollisionInfo.scale.y = currentCollisionInfo.localToWorld.GetColumn(1).magnitude;

                    currentCollisionInfo.pos = currentCollider.transform.position;

                    currentCollisionInfo.numCollisions = 1;
                    currentCollisionInfo.collidingNodes[0] = i;


                    switch (currentCollider)
                    {
                        case CircleCollider2D c:
                            currentCollisionInfo.type = ColliderType.Circle;
                            currentCollisionInfo.size.x = currentCollisionInfo.size.y = c.radius;
                            break;
                        case BoxCollider2D b:
                            currentCollisionInfo.type = ColliderType.Box;
                            currentCollisionInfo.size = b.size;
                            break;
                        default:
                            currentCollisionInfo.type = ColliderType.None;
                            break;
                    }

                    numCollisions++;

                    if (numCollisions >= maxCollisions)
                    {
                        return;
                    } else
                    {

                        //CollisionInfo currentCollisionInfo = collisionsInfo[idx];


                    }

                }

            }


        }

    }


    void GenerateTentacle()
    {
        float firstCenter = origin.x;

        Point firstLeftPoint = new Point(new Vector2(firstCenter - halfSegment, origin.y), new Vector2(firstCenter - halfSegment, origin.y));

        points.AddLast(firstLeftPoint);

        for (int i = 1; i < numberOfPoints; i++)
        {


            float center = segmentLength * i + origin.x;

            Point rightPoint = new Point(new Vector2(center + halfSegment, origin.y), new Vector2(center + halfSegment, origin.y));

            points.AddLast(rightPoint);

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

        LinkedListNode<Point> currentNode = points.First;
        LinkedListNode<Point> nextNode = currentNode.Next;


        while (nextNode != null)
        {
            Vector2 currentPos = currentNode.Value.currentPosition;
            Vector2 nextPos = nextNode.Value.currentPosition;

            Vector2 segmentOrientation = (nextPos - currentPos).normalized;

            float separation = Vector2.Distance(currentPos, nextPos);
            float error = separation - segmentLength;


            //Debug.Log($"CurrentPos: {currentPos}, NextPos: {nextPos}, SegmentOrientation: {segmentOrientation}, Separation: {separation}, Error: {error}, FixedCurrent: {fixedCurrent}, FixedNext: {fixedNext}");
            //Debug.Log(currentNode.Value.id + ":" + separation);

            if (!currentNode.Value.locked)
            {
                Vector2 fixedCurrent = currentPos + segmentOrientation * (error * 0.5f);
                currentNode.Value.currentPosition = fixedCurrent;
            }

            if (!nextNode.Value.locked)
            {
                Vector2 fixedNext = nextPos - segmentOrientation * (error * 0.5f);
                nextNode.Value.currentPosition = fixedNext;
            }


            currentNode = nextNode;
            nextNode = currentNode.Next;
        }
        //Debug.Log("----------");
    }
    private void Collide()
    {
        foreach (Point currentPoint in points)
        {

            bool isInsideCollider = Physics2D.OverlapPoint(currentPoint.currentPosition);

            if (isInsideCollider)
            {

                currentPoint.currentPosition = currentPoint.previousPosition;

            }

        }
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

    void SimulateTentacle()
    {
        if (Input.GetMouseButton(0))
        {
            points.First().currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //points.First().currentPosition = new Vector2(-1, -1);
            points.First().locked = true;
        }
        else
        {
            points.First().locked = false;
        }

        MoveSimulation();

        FirstAndLastDistanceConstraint();
        for (int i = 0; i < distanceCheckIterations; i++)
        {
            DistanceConstraint();
        }


        Collide();
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
            }


        }

    }
}