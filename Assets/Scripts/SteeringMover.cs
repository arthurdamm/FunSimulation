using UnityEngine;

public class SteeringMover : MonoBehaviour
{
    [Header("Motion")]
    [SerializeField] float maxSpeed = 6f;
    [SerializeField] float maxAccel = 12f;

    private bool loggedCircle = false;
    private bool loggedDefault = false;

    Vector3 velocity;

    FaceVelocity face;

    void Awake()
    {
        face = GetComponent<FaceVelocity>();
    }

    void Update()
    {
        Vector3 desiredVelocity = (orbitCenter == null ? ComputeDesiredVelocity() : ComputeDesiredVelocityCircle());
        Vector3 steer = desiredVelocity - velocity;

        // Limit acceleration
        steer = Vector3.ClampMagnitude(steer, maxAccel);

        velocity += steer * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        transform.position += velocity * Time.deltaTime;

        if (face) face.Velocity = velocity;
    }

    Vector3 ComputeDesiredVelocity()
    {
        if (!loggedDefault)
        {
            loggedDefault = true;
            Debug.Log("Compute default");
        }
        // placeholder: gentle wander (we’ll improve)
        Vector3 dir = transform.forward;
        dir = Quaternion.AngleAxis(Random.Range(-20f, 20f) * Time.deltaTime, Vector3.up) * dir;
        return dir.normalized * maxSpeed;
    }
    
    public Transform orbitCenter;
    public float orbitRadius = 10f;
    public float orbitSpeed = 6f;
    public float radialCorrection = 2f; // how strongly it stays near radius

    Vector3 ComputeDesiredVelocityCircle()
    {
        if (!loggedCircle)
        {
            loggedCircle = true;
            Debug.Log("Compute circle");
        }
        Vector3 toCenter = transform.position - orbitCenter.position;
        toCenter.y = 0f;

        float dist = toCenter.magnitude;
        if (dist < 0.001f)
            return transform.forward * orbitSpeed;

        Vector3 radialDir = toCenter / dist;

        // Tangent direction (left-hand orbit)
        Vector3 tangent = Vector3.Cross(Vector3.up, radialDir);

        // Maintain radius (push in or out)
        float radiusError = dist - orbitRadius;
        Vector3 correction = -radialDir * radiusError * radialCorrection;

        Vector3 desired = tangent * orbitSpeed + correction;

        return desired;
    }
}