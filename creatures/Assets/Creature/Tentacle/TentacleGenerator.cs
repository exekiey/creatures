using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public struct Segment
{

    public Vector2 from;
    public Vector2 to;
    public GameObject segmentObject;

    public Segment(Vector2 from, Vector2 to, GameObject segmentObject)
    {
        this.from = from;
        this.to = to;
        this.segmentObject = segmentObject;
    }
}

public class TentacleGenerator : MonoBehaviour
{

    [SerializeField] int numberOfSegments;
    [SerializeField] float tentacleLength;
    GameObject originObject;

    Rigidbody2D rigidbody2D;

    float segmentLength;
    Vector2 origin;

    [SerializeField] float tentacleWidth;

    List<Segment> segments;

    HingeJoint2D hingeJoint2D;


    [SerializeField] GameObject segmentPrefab;

    [SerializeField] SuckerScript sucker;

    // Start is called before the first frame update
    void Awake()
    {
        segmentLength = tentacleLength / numberOfSegments;
        segments = new List<Segment>(numberOfSegments);
        originObject = gameObject.transform.parent.gameObject;
        origin = originObject.transform.position;

        hingeJoint2D = GetComponent<HingeJoint2D>();

        rigidbody2D = GetComponentInParent<Rigidbody2D>();

    }

    private void Start()
    {
        GenerateTentacle();
    }

    void GenerateTentacle()
    {



        sucker.Distance = tentacleLength;

        float solapationDistance = segmentLength / 5;

        Vector2 currentOrigin = origin;
        Vector2 firstDestination = currentOrigin + new Vector2(segmentLength, 0);

        Vector2 firstFixedPosition = new Vector2(currentOrigin.x + segmentLength / 2, origin.y);

        firstFixedPosition = firstFixedPosition - new Vector2(solapationDistance, 0);

        GameObject firstSegmentPrefab = Instantiate(segmentPrefab, firstFixedPosition, Quaternion.identity);

        HingeJoint2D firstHingeJoint = firstSegmentPrefab.GetComponent<HingeJoint2D>();
        firstHingeJoint.anchor = new Vector2(-0.5f, 0);
        firstHingeJoint.connectedBody = rigidbody2D;

        CustomStrictDistanceJoint firstDistanceJoint = firstSegmentPrefab.GetComponent<CustomStrictDistanceJoint>();
        firstDistanceJoint.distance = 0.5f;
        firstDistanceJoint.connectedBody = rigidbody2D;
        firstDistanceJoint.connectedAnchor = new Vector2(segmentLength / 2, 0);
        firstDistanceJoint.anchor = new Vector2(-segmentLength / 2, 0);


        firstSegmentPrefab.name = "0";

        firstSegmentPrefab.transform.localScale = new Vector2(segmentLength, tentacleWidth);

        Segment firstSegment = new Segment(currentOrigin, firstDestination, firstSegmentPrefab);

        segments.Insert(0, firstSegment);

        currentOrigin = firstDestination;

        for (int i = 1; i < numberOfSegments; i++)
        {

            Vector2 currentDestination = currentOrigin + new Vector2(segmentLength, 0);

            Vector2 fixedPosition = new Vector2(currentOrigin.x + segmentLength / 2, origin.y);

            fixedPosition = fixedPosition - new Vector2(solapationDistance, 0);

            GameObject currentSegmentPrefab = Instantiate(segmentPrefab, fixedPosition, Quaternion.identity);

            currentSegmentPrefab.GetComponent<SpriteRenderer>().color = Random.ColorHSV();

            currentSegmentPrefab.name = i.ToString();

            currentSegmentPrefab.transform.localScale = new Vector2(segmentLength, tentacleWidth);

            GameObject previousSegmentPrefab = segments[i - 1].segmentObject;

            Rigidbody2D previousRigidbody = previousSegmentPrefab.GetComponent<Rigidbody2D>();

            HingeJoint2D currentHingeJoint = currentSegmentPrefab.GetComponent<HingeJoint2D>();
            currentHingeJoint.anchor = new Vector2(-0.5f, 0);
            currentHingeJoint.connectedBody = previousRigidbody;

            CustomStrictDistanceJoint currentDistanceJoint = currentSegmentPrefab.GetComponent<CustomStrictDistanceJoint>();
            currentDistanceJoint.distance = 0.5f;
            currentDistanceJoint.connectedBody = previousRigidbody;
            currentDistanceJoint.connectedAnchor = new Vector2(segmentLength / 2, 0);
            currentDistanceJoint.anchor = new Vector2(-segmentLength / 2, 0);

            segments.Insert(i, new Segment(currentOrigin, currentDestination, currentSegmentPrefab));
            currentOrigin = currentDestination;


        }

        
        HingeJoint2D suckerTentacleHingeJoint2D = sucker.TentacleJoint;
        suckerTentacleHingeJoint2D.anchor = new Vector2(-0.5f, 0);
        suckerTentacleHingeJoint2D.connectedBody = segments.Last<Segment>().segmentObject.GetComponent<Rigidbody2D>();

        Vector2 suckerFixedPosition = new Vector2(currentOrigin.x + sucker.Object.transform.localScale.x / 2, origin.y);
        suckerTentacleHingeJoint2D.transform.position = suckerFixedPosition;



        hingeJoint2D.connectedBody = segments[0].segmentObject.GetComponent<Rigidbody2D>();
        hingeJoint2D.anchor = new Vector2(0.5f, 0);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
