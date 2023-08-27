using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

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

    LineRenderer lineRenderer;
    EdgeCollider2D edgeCollider2D;

    float segmentLength;
    private float halfSegment;
    Vector2 origin;

    [SerializeField] float tentacleWidth;

    LinkedList<Point> points;

    Point last;

    [SerializeField] GameObject segmentPrefab;

    [SerializeField] SuckerScript sucker;
    [SerializeField] private bool followCursor;

    // Start is called before the first frame update
    void Awake()
    {

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numberOfPoints;

        edgeCollider2D = GetComponent<EdgeCollider2D>();

        segmentLength = tentacleLength / (numberOfPoints + 1);

        points = new LinkedList<Point>();

        origin = transform.position;

        GenerateTentacle();

        points.First().locked = false;


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

            if (i == numberOfPoints - 1)
            {
                last = rightPoint;
            }

        }


    }


    private void MoveSimulation()
    {


        foreach (Point currentPoint in points)
        {

            if (!currentPoint.locked)
            {

                Vector2 positionBeforeUpdate = currentPoint.currentPosition;

                Vector2 currentVelocity = currentPoint.currentPosition - currentPoint.previousPosition;

                currentPoint.currentPosition += currentVelocity;

                currentPoint.previousPosition = positionBeforeUpdate;

                //currentPoint.currentPosition += Vector2.down * gravity * Time.deltaTime;
                currentPoint.currentPosition += Vector2.down * gravity * Time.deltaTime * Time.deltaTime;
            }

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

        float separation = Vector2.Distance(first.currentPosition, last.currentPosition);

        if (first.locked && separation > tentacleLength)
        {

            Vector2 orientation = (first.currentPosition - last.currentPosition).normalized;

            float error = separation - tentacleLength;


            Vector2 correctionVector = orientation * error;

            last.currentPosition += correctionVector;
            return;
        }

        if (last.locked && separation > tentacleLength)
        {

            Vector2 orientation = (first.currentPosition - last.currentPosition).normalized;

            float error = separation - tentacleLength;


            Vector2 correctionVector = orientation * error;

            first.currentPosition -= correctionVector;
            return;

        }


    }

    void SimulateTentacle()
    {
        if (Input.GetMouseButton(0))
        {
            points.First().currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            points.First().locked = true;
        } else
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
                if (point == last)
                {

                    Gizmos.DrawIcon(point.currentPosition, "sv_icon_dot6_pix16_gizmo");
                } else
                {
                    Gizmos.DrawIcon(point.currentPosition, "sv_icon_dot1_pix16_gizmo");
                }
            }


        }

    }
}