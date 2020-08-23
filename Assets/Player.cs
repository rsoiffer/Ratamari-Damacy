using UnityEngine;

public class Player : MonoBehaviour
{
    public float torqueMul = 1;
    public float reorientTorque = 1;
    public float maxVelocity = 100;

    public float sensitivity = 5.0f;
    public float smoothing = 2.0f;
    public Transform camera;
    public float cameraDistance;

    private Rigidbody myRigidbody;
    private Vector2 mouseLook;
    private Vector2 smoothV;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myRigidbody.maxAngularVelocity = maxVelocity;
    }

    void FixedUpdate()
    {
        var vertical = Input.GetAxis("Vertical");
        myRigidbody.AddTorque(vertical * torqueMul * camera.right);

        var horizontal = Input.GetAxis("Horizontal");
        myRigidbody.AddTorque(horizontal * torqueMul * Vector3.Cross(Vector3.up, camera.right));

        // var angVelProjBad = Vector3.ProjectOnPlane(myRigidbody.angularVelocity, transform.right);
        // myRigidbody.AddTorque(-reorientTorque * angVelProjBad);
        var cross = Vector3.Cross(transform.right, myRigidbody.angularVelocity);
        myRigidbody.AddTorque(reorientTorque * cross);
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

        camera.position = transform.position - cameraDistance * (rotation * Vector3.forward);
        camera.rotation = rotation;
    }
}