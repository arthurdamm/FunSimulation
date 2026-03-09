using UnityEngine;

public class SteeringMover : MonoBehaviour
{
    [Header("Motion")]
    [SerializeField] protected float maxSpeed = 6f;
    [SerializeField] protected float maxAccel = 12f;
    [SerializeField] protected bool clockWise = true;
    
    protected bool loggedCircle = false;
    protected bool loggedDefault = false;

    protected Vector3 velocity;

    protected FaceVelocity face;

    void Awake()
    {
        face = GetComponent<FaceVelocity>();
    }

    void Update()
    {
        // Debug.Log($"Mover t: {transform.gameObject.name}");
        
        Vector3 desiredVelocity = (!orbitCenter ? ComputeDesiredVelocity() : ComputeDesiredVelocityCircle());
        // Vector3 desiredVelocity = ComputeDesiredVelocity();
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
            // Debug.Log("Compute default");
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
            // Debug.Log("Compute circle");
        }
        Vector3 toCenter = transform.position - orbitCenter.position;
        // toCenter.y = 0f;

        float dist = toCenter.magnitude;
        if (dist < 0.001f)
            return transform.forward * orbitSpeed;

        Vector3 radialDir = toCenter / dist;

        // Tangent direction (left-hand orbit)
        Vector3 tangent = Vector3.Cross(clockWise ? Vector3.up : Vector3.down, radialDir);

        // Maintain radius (push in or out)
        float radiusError = dist - orbitRadius;
        Vector3 correction = -radialDir * radiusError * radialCorrection;

        Vector3 desired = tangent * orbitSpeed + correction;

        return desired;
    }
    
    // Call this inside your steering computation (NOT using rigidbodies).
    // Vector3 ComputeSeparation(Transform me, IList<Transform> agents, float radius, float strength)
    // {
    //     float r2 = radius * radius;
    //     Vector3 push = Vector3.zero;
    //     int count = 0;
    //
    //     for (int i = 0; i < agents.Count; i++)
    //     {
    //         Transform other = agents[i];
    //         if (other == me) continue;
    //
    //         Vector3 d = me.position - other.position;
    //         float dist2 = d.sqrMagnitude;
    //         if (dist2 <= 0f || dist2 > r2) continue;
    //
    //         // Weight: stronger when closer. (dist2 in denom avoids sqrt)
    //         push += d / dist2;
    //         count++;
    //     }
    //
    //     if (count == 0) return Vector3.zero;
    //
    //     push /= count;
    //     return push.normalized * strength;
    // }
}