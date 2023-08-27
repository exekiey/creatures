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
        lineRenderer.positionCount = numberOfSegments;

        edgeCollider2D = GetComponent<EdgeCollider2D>();

        segmentLength = tentacleLength / numberOfSegments;
        halfSegment = segmentLength / 2;
        segments = new List<Segment>(numberOfSegments);

        points = new List<Point>();

        Debug.Log(halfSegment);

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

                currentPoint.currentPosition += Vector2.down * gravity * Time.deltaTime;
                //currentPoint.currentPosition += Vector2.down * gravity * Time.deltaTime * Time.deltaTime;
            }

        }

        Point lastPoint = points.Last();

        //lastPoint.currentPosition += Vector2.down * gravity * Time.deltaTime;


        for (int i = 0;i < distanceCheckIterations;i++)
        {

            foreach (Segment currentSegment in segments)
            {


                Vector2 positionA = currentSegment.pointA.currentPosition;
                Vector2 positionB = currentSegment.pointB.currentPosition;

                Vector2 center = (positionA + positionB) / 2;
                Vector2 orientation = (positionB - positionA).normalized;

                if (!currentSegment.pointA.locked)
                {
                    currentSegment.pointA.currentPosition = center - orientation * halfSegment;
                }

                if (!currentSegment.pointB.locked)
                {
                    currentSegment.pointB.currentPosition = center + orientation * halfSegment;
                }

            }

        }





    }

    void DrawLine()
    {
        Vector3[] positions = new Vector3[numberOfSegments];

        for (int i = 0; i < numberOfSegments; i++)
        {

            Vector3 currentPosition = segments[i].pointA.currentPosition;

            positions[i] = currentPosition;

        }

        Vector3 lastSegmentPointB = segments[numberOfSegments - 1].pointB.currentPosition;

        positions[numberOfSegments - 1] = lastSegmentPointB;

        lineRenderer.SetPositions(positions);

    }

    private void PlaceCollider()
    {

        List<Vector2> points = new List<Vector2>(numberOfSegments);

        for (int i = 0; i < numberOfSegments; i++)
        {

            Vector3 currentPosition = segments[i].pointA.currentPosition;
            points.Insert(i, currentPosition);

        }


        Vector3 lastSegmentPointB = segments[numberOfSegments - 1].pointB.currentPosition;
        points.Insert(numberOfSegments - 1, lastSegmentPointB);

        edgeCollider2D.SetPoints(points);

    }

    // Update is called once per frame
    void Update()
    {
        SimulateTentacle();

        DrawLine();

        PlaceCollider();
    }



    private void OnDrawGizmos()
    {

        if (points != null)
        {

            foreach (Point point in points)
            {

                Gizmos.DrawIcon(point.currentPosition, "sv_icon_dot1_pix16_gizmo");

            }
        }

    }
}
