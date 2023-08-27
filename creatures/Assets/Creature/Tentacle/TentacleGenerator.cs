using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class TentacleGenerator : MonoBehaviour
{
    public class Point
    {

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
        }

    }

    public struct Segment
    {

        public Point pointA;
        public Point pointB;

        public Segment(Point pointA, Point pointB)
        {

            this.pointA = pointA;

            this.pointB = pointB;

        }
    }

    [SerializeField] int numberOfSegments;
    [SerializeField] float tentacleLength;
    [SerializeField] int distanceCheckIterations = 50;
    [SerializeField] float gravity = 10f;
    GameObject originObject;

    LineRenderer lineRenderer;
    EdgeCollider2D edgeCollider2D;

    float segmentLength;
    private float halfSegment;
    Vector2 origin;

    [SerializeField] float tentacleWidth;

    public List<Segment> segments;
    List<Point> points;


    [SerializeField] GameObject segmentPrefab;

    [SerializeField] SuckerScript sucker;

    // Start is called before the first frame update
    void Awake()
    {

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numberOfSegments + 1;

        edgeCollider2D = GetComponent<EdgeCollider2D>();

        segmentLength = tentacleLength / numberOfSegments;
        halfSegment = segmentLength / 2;
        segments = new List<Segment>(numberOfSegments);

        points = new List<Point>();

        GenerateTentacle();

        segments[0].pointA.locked = true;

    }

    void GenerateTentacle()
    {
        float firstCenter = origin.x;

        Point firstLeftPoint = new Point(new Vector2(firstCenter - halfSegment, origin.y), new Vector2(firstCenter - halfSegment, origin.y));
        Point firstRightPoint = new Point(new Vector2(firstCenter + halfSegment, origin.y), new Vector2(firstCenter + halfSegment, origin.y));

        points.Add(firstLeftPoint);


        Segment firstSegment = new Segment(firstLeftPoint, firstRightPoint);

        segments.Insert(0, firstSegment);
        for (int i = 1; i < numberOfSegments; i++)
        {


            float center = segmentLength * i + origin.x;

            Point leftPoint = segments[i - 1].pointB;
            Point rightPoint = new Point(new Vector2(center + halfSegment, origin.y), new Vector2(center + halfSegment, origin.y));

            points.Add(leftPoint);

            if (i == numberOfSegments - 1)
            {
                points.Add(rightPoint);
            }

            Segment currentSegment = new Segment(leftPoint, rightPoint);

            segments.Insert(i, currentSegment);

        }


    }

    void SimulateTentacle()
    {
        
        foreach (Point currentPoint in points)
        {

            if (!currentPoint.locked)
            {

                Vector2 previousPosition = currentPoint.currentPosition;

                Vector2 currentVelocity = currentPoint.currentPosition - currentPoint.previousPosition;

                currentPoint.currentPosition += currentVelocity;

                currentPoint.previousPosition = previousPosition;

                //currentPoint.currentPosition += Vector2.down * gravity * Time.deltaTime;
                currentPoint.currentPosition += Vector2.down * gravity * Time.deltaTime * Time.deltaTime;
            }

        }

        Point lastPoint = points.Last();

        if (Input.GetMouseButton(0))
        {
            lastPoint.currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //lastPoint.locked = true;
        }


        //lastPoint.currentPosition += Vector2.down * gravity * Time.deltaTime;
           

        for (int i = 1; i < points.Count; i++)
        {
            Vector2 currentPointPos = points[i].currentPosition;
            Vector2 previousPointPos = points[i-1].currentPosition;

            Vector2 segmentOrientation = (currentPointPos - previousPointPos).normalized;

            float distance = Vector2.Distance(previousPointPos, segmentOrientation);

            float error = distance - segmentLength;

            points[i].currentPosition = segmentOrientation * error;
        }

        foreach (Point currentPoint in points)
        {

            bool isInsideCollider = Physics2D.OverlapPoint(currentPoint.currentPosition);

            if (isInsideCollider)
            {

                currentPoint.currentPosition = currentPoint.previousPosition;


            }

        }
    }



    void DrawLine()
    {
        Vector3[] positions = new Vector3[numberOfSegments + 1];


        Debug.Log(positions.Length);
        for (int i = 0; i < numberOfSegments; i++)
        {

            Vector3 currentPosition = segments[i].pointA.currentPosition;

            positions[i] = currentPosition;

        }

        
        Vector3 lastSegmentPointB = segments.Last().pointB.currentPosition;

        positions[numberOfSegments] = lastSegmentPointB;

        lineRenderer.SetPositions(positions);

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

                Gizmos.DrawIcon(point.currentPosition, "sv_icon_dot1_pix16_gizmo");

            }

            Gizmos.DrawIcon(points.Last().currentPosition, "sv_icon_dot6_pix16_gizmo");

        }

    }
}
