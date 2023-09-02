using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tentacle))]
public class TentacleAttachBody : MonoBehaviour
{

    [SerializeField] GameObject attachedObject;
    Tentacle tentacle;

    Rigidbody2D attachedBody;

    // Start is called before the first frame update
    void Awake()
    {
        attachedBody = attachedObject.GetComponent<Rigidbody2D>();
        tentacle = GetComponent<Tentacle>();
    }

    private void Start()
    {
        tentacle.Points[0].locked = true;
    }

    // Update is called once per frame
    void Update()
    {

        tentacle.Points[0].currentPosition = attachedBody.transform.position;

    }
}
