using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CustomStrictDistanceJoint : MonoBehaviour
{

    [SerializeField] Vector2 _anchor;
    Vector2 _worldAnchorPosition;
    [SerializeField] Vector2 _connectedAnchor;
    Vector2 _worldConnectedAnchorPosition;
    [SerializeField] float _distance;
    [SerializeField] float correctionForce;

    [SerializeField] Rigidbody2D _connectedBody;
    Rigidbody2D _rigidbody2D;



    Vector2 _previousPosition;
    Vector2 _previousConnectedPosition;

    HingeJoint2D hin;

    public float distance { get => _distance; set => _distance = value; }
    public Rigidbody2D connectedBody { get => _connectedBody; set => _connectedBody = value; }
    public Vector2 connectedAnchor { get => _connectedAnchor; set => _connectedAnchor = value; }
    public Vector2 anchor { get => _anchor; set => _anchor = value; }

    // Start is called before the first frame update
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Start()
    {

        _worldAnchorPosition = transform.position + (Vector3)_anchor;
        _worldConnectedAnchorPosition = _connectedBody.transform.position + (Vector3)_connectedAnchor;
        _previousPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _worldAnchorPosition = transform.position + (Vector3)_anchor;
        _worldConnectedAnchorPosition = _connectedBody.transform.position + (Vector3)_connectedAnchor;

        float currentDistance = Vector2.Distance(_worldAnchorPosition, _worldConnectedAnchorPosition);

        if (currentDistance > _distance)
        {

            gameObject.GetComponent<SpriteRenderer>().color = Color.red;

            float separation = currentDistance - _distance;

            Vector2 correctionDirection = transform.position - connectedBody.transform.position;
            Vector2 correctionForceVector = correctionDirection * separation * correctionForce;

            _rigidbody2D.AddForce(-correctionForceVector);
            _connectedBody.AddForce(correctionForceVector);


        } else
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        }

        _previousPosition = transform.position;
        _previousConnectedPosition = connectedBody.transform.position;

    }

    private void OnDrawGizmos()
    {

        _worldAnchorPosition = transform.position + (Vector3)_anchor;
        _worldConnectedAnchorPosition = _connectedBody.transform.position + (Vector3)_connectedAnchor;
        Gizmos.DrawIcon(_worldAnchorPosition, "sv_icon_dot1_pix16_gizmo", true);


        Gizmos.DrawIcon(_worldConnectedAnchorPosition, "sv_icon_dot6_pix16_gizmo", true);

        if (Vector3.Distance(_worldAnchorPosition, _worldConnectedAnchorPosition) > distance)
        {

            Gizmos.color = Color.red;

        } else
        {
            Gizmos.color = Color.green;
        }

        Gizmos.DrawLine(_worldAnchorPosition, _worldConnectedAnchorPosition);


    }
}
