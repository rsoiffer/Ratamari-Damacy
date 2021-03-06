﻿using UnityEngine;

public class Player : MonoBehaviour
{
    public float torqueMul = .5f;
    public float reorientTorque = -.05f;
    public float maxVelocity = 20;
    public float jumpImpulse = 5;

    public float sensitivity = 2;
    public float smoothing = 2;
    public Transform myCamera;
    public float cameraDistance = 2;
    public float cameraSmoothing = .1f;

    public float maxGrabMassRatio = .2f;
    public float totalMass = 1;

    [HideInInspector]
    public Rigidbody myRigidbody;
    private SphereCollider myCollider;
    private float colliderInitialRadius;
    private Vector2 mouseLook;
    private Vector2 smoothV;
    private Vector3 camPos;
    private Vector3 camVel;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myRigidbody.maxAngularVelocity = maxVelocity;
        myCollider = GetComponent<SphereCollider>();
        colliderInitialRadius = myCollider.radius;
    }

    void FixedUpdate()
    {
        var vertical = Input.GetAxis("Vertical");
        myRigidbody.AddTorque(vertical * totalMass * totalMass * torqueMul * myCamera.right);

        var horizontal = Input.GetAxis("Horizontal");
        myRigidbody.AddTorque(horizontal * totalMass * totalMass * torqueMul * Vector3.Cross(Vector3.up, myCamera.right));

        // var angVelProjBad = Vector3.ProjectOnPlane(myRigidbody.angularVelocity, transform.right);
        // myRigidbody.AddTorque(-reorientTorque * angVelProjBad);
        var cross = Vector3.Cross(transform.right, myRigidbody.angularVelocity);
        myRigidbody.AddTorque(reorientTorque * totalMass * totalMass * cross);

        myRigidbody.mass = totalMass;
        myCollider.radius = colliderInitialRadius * Mathf.Pow(totalMass, .5f);
    }

    void Update()
    {
        var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);
        mouseLook += smoothV;
        mouseLook.y = Mathf.Clamp(mouseLook.y, -70, 70);

        var rotation = Quaternion.Euler(-mouseLook.y, mouseLook.x, 0);
        var layers = Physics.DefaultRaycastLayers - (1 << 8);
        var actualCameraDistance = cameraDistance * Mathf.Pow(totalMass, .5f);
        var dist = Physics.Raycast(transform.position,
            rotation * -Vector3.forward, out var hit, actualCameraDistance, layers)
            ? hit.distance
            : actualCameraDistance;
        var goalPos = transform.position;
        camPos = Vector3.SmoothDamp(camPos, goalPos, ref camVel, cameraSmoothing);

        myCamera.position = camPos - dist * (rotation * Vector3.forward);
        myCamera.rotation = rotation;

        if (Input.GetButtonDown("Jump") && OnGround)
        {
            myRigidbody.AddForce(totalMass * jumpImpulse * Vector3.up, ForceMode.Impulse);
        }
    }

    void OnCollisionEnter(Collision other)
    {
    }

    private bool OnGround => Physics.SphereCast(
        transform.position, myCollider.radius, Vector3.down, out var hit, .1f);
}