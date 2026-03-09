using UnityEngine;

public class SteeringMoverRing : SteeringMover
{
    void Update()
    {
        Debug.Log($"Mover t: {transform.gameObject.name}");
        
        Vector3 desiredVelocity = (!orbitCenter ? ComputeDesiredVelocity() : ComputeDesiredVelocityCircle());
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

    Vector3 ComputeDesiredVelocityCircle()
    {
        if (!loggedCircle)
        {
            loggedCircle = true;
            Debug.Log("Compute circle");
        }
        Vector3 toCenter = transform.position - orbitCenter.position;
        // toCenter.y = 0f;

        float dist = toCenter.magnitude;
        if (dist < 0.001f)
            return transform.forward * orbitSpeed;

        Vector3 radialDir = toCenter / dist;

        // Tangent direction (left-hand orbit)
        // Vector3 tangent = Vector3.Cross(clockWise ? Vector3.up : Vector3.down, radialDir);
        Vector3 tangent = Vector3.Cross(Vector3.right, radialDir);

        // Maintain radius (push in or out)
        float radiusError = dist - orbitRadius;
        Vector3 correction = -radialDir * radiusError * radialCorrection;

        Vector3 desired = tangent * orbitSpeed + correction;

        return desired;
    }
}