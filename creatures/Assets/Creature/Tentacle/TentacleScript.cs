using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class TentacleScript : MonoBehaviour
{

    LineRenderer lineRenderer;
    EdgeCollider2D edgeCollider;


    List<TentacleSegment> segments = new List<TentacleSegment>();
    float segmentLen = 0.25f;
    int numberOfSegments = 10;

    float lineWidth = 0.1f;

    [SerializeField] Vector2 gravity = new Vector2(0, -1);
    private List<Vector2> colliderPoints;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();

        Vector2 startPoint = transform.position;

        for (int i = 0; i < numberOfSegments; i++)
        {

            segments.Add(new TentacleSegment(startPoint));
            startPoint.y -= segmentLen;

        }


    }

    void DrawTentacle()
    {

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] tentaclePositions = new Vector3[numberOfSegments];
        colliderPoints = new List<Vector2>(numberOfSegments);

        for (int i = 0; i < numberOfSegments; i++)
        {

            Vector2 pos1 = segments[i].pos1;

            tentaclePositions[i] = pos1;

            Vector2 fixedPosition = pos1 - (Vector2)transform.position;

            colliderPoints.Insert(i, fixedPosition);


        }

        lineRenderer.positionCount = tentaclePositions.Length;

        lineRenderer.SetPositions(tentaclePositions);

        edgeCollider.SetPoints(colliderPoints);

    }

    void Simulate()
    {

        for (int i = 1; i < numberOfSegments; i++)
        {

            TentacleSegment firstSegment = segments[i];
            Vector2 velocity = firstSegment.pos1 - firstSegment.pos0;
            firstSegment.pos0 = firstSegment.pos1;
            firstSegment.pos1 += velocity;

            firstSegment.pos1 += gravity * Time.deltaTime;

            segments[i] = firstSegment;
        }

    }

    void Constraints()
    {

        TentacleSegment firstSegment = segments[0];

        firstSegment.pos1 = transform.position;

        segments[0] = firstSegment;

        for (int i = 0; i < numberOfSegments - 1; i++)
        {

            TentacleSegment first = segments[i];
            TentacleSegment second = segments[i+1];

            float distance = (first.pos1 - second.pos1).magnitude;

            float correctionAmount = Mathf.Abs(distance - segmentLen);
            Vector2 correctionDirection = Vector2.zero;

            if (distance > segmentLen)
            {

                correctionDirection = (first.pos1 - second.pos1).normalized;

            } else if (distance < segmentLen){

                correctionDirection = (second.pos1 - first.pos1).normalized;
            }

            Vector2 correctionVector = correctionDirection * correctionAmount;

            if (i != 0)
            {

                first.pos1 -= correctionVector * 0.5f;
                segments[i] = first;
                second.pos1 += correctionVector * 0.5f;
                segments[i+1] = second;

            } else
            {

                second.pos1 += correctionVector;
                segments[i+1] = second;

            }

        }

    }

    private void FixedUpdate()
    {
        Simulate();
        for (int i = 0; i < 1; i++)
        {
            Constraints();
        }

    }

    private void Update()
    {

        DrawTentacle();
    }

    public struct TentacleSegment
    {

        public Vector2 pos0;
        public Vector2 pos1;

        public TentacleSegment(Vector2 pos)
        {
            this.pos0 = pos;
            this.pos1 = pos;
        }

    }

}
