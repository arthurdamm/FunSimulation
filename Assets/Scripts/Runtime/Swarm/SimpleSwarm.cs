using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class SimpleSwarm : MonoBehaviour
{
    public GameObject shipPrefab;     // sphere prefab
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private GameObject formationObj;
    [SerializeField] private ImprovedBeam beamPrefab;
    [SerializeField] private Transform orbitCenter;
    [SerializeField] private SimpleSwarm enemySwarm;
    
    [SerializeField] private float smellStrength = 1f;
    [SerializeField] private float epsilon = 1f;

    [Header("Motion")]
    [SerializeField] protected float maxSpeed = 6f;
    [SerializeField] protected float maxAccel = 12f;
    [SerializeField] protected bool clockWise = true;
    [SerializeField] private float maxTurnAngle = 45f;
    
    [SerializeField] float turnSpeedDegPerSec = 360f;

    // for the push-out separation force
    [SerializeField] private float separationRadius = 3f;
    [SerializeField] private float separationStrength = 2.5f;
    
    public float orbitRadius = 10f;
    public float orbitSpeed = 6f;
    public float radialCorrection = 2f; // how strongly it stays near radius
    
    public int count = 50;
    public float areaSize = 10f;

    public float beamCooldown = 1f;

    private Vector3[] velocities;
    private float[] previousSmells;
    private float[] lastBeamFiredTimes;
    private bool[] destroyed;

    public List<Transform> agents = new();
    private List<Transform> foods = new();

    private SpatialHashGrid3D spatialGrid = new(10f);

    private MeshMover meshMover = new();
    private List<Vector3> meshMoverPositions;
    public float debugSphereRadius = 1f;

    void handleShipDeath(int agentId)
    {
        Debug.Log($"handleShipDeath {agentId}");
        destroyed[agentId] = true;
    }

    void Awake()
    {
        velocities = new Vector3[count];
        previousSmells = new float[count];
        lastBeamFiredTimes = new float[count];
        destroyed = new bool[count];

        Debug.Log($"Start Time: {Time.time}");
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = Random.insideUnitSphere * areaSize;
            pos.y += 10f;
            GameObject go = Instantiate(shipPrefab, pos, Quaternion.identity);
            go.transform.parent = transform;
            SteeringMover sm = go.GetComponent<SteeringMover>();
            if (sm)
                sm.orbitCenter = orbitCenter;
            agents.Add(go.transform);
            
            velocities[i] = Random.onUnitSphere;
            // velocities[i].y = 0f;
            lastBeamFiredTimes[i] = Time.time;
        }

        meshMover.gameObj = formationObj;
        meshMover.OnEnable();
        meshMoverPositions = meshMover.GetPositions(count);
    }

    void Update()
    {
        for (int i = 0; i < agents.Count; i++)
        {
            if (destroyed[i])
            {
                continue;
            }
            
            Transform t = agents[i];
            // Debug.Log($"t: {t.gameObject.name}");
            
            // t.position += velocities[i] * (speed * Time.deltaTime);
            
            // if (Time.deltaTime % i == 0)

            SteerTowardsDesiredVelocity(i);
            FaceVelocity(i);
            TryToFire(t, i);
        }
    }

    private void OnDrawGizmos()
    {
        foreach (var position in meshMoverPositions)
        {
            Gizmos.color = Color.brown;
            Gizmos.DrawSphere(position, debugSphereRadius);
        }
    }

    private void TryToFire(Transform source, int agentIndex)
    {
        if (Time.time - lastBeamFiredTimes[agentIndex] < beamPrefab.cooldown + beamPrefab.lifetime)
        {
            return;
        }
        
        foreach (Transform enemyAgent in enemySwarm.agents)
        {
            if ((enemyAgent.position - source.position).sqrMagnitude < beamPrefab.range * beamPrefab.range)
            {
                lastBeamFiredTimes[agentIndex] = Time.time;
                ImprovedBeam beam = Instantiate(beamPrefab);
                beam.Fire(source, enemyAgent);
            }
        }
    }
    
    public void TrySpawnFood(Vector3 pos)
    {
        GameObject food = Instantiate(foodPrefab, pos, Quaternion.identity);
        foods.Add(food.transform);
        orbitCenter = food.transform;
    }

    public void MoveOrbitCenter(Vector3 pos)
    {
        orbitCenter.transform.position = pos + Vector3.up * 4;
    }

    float GetFoodSmellStrength(Vector3 from)
    {
        float smell = 0;
        foreach (var food in foods)
        {
            float d = Vector3.Distance(from, food.position);
            smell += smellStrength / (d * d + epsilon);
        }

        return smell;
    }
    
    void FaceVelocity(int agentIndex)
    {
        Vector3 v = velocities[agentIndex];
        // v.y = 0f; // keep ships level for now

        if (v.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(v.normalized, Vector3.up);
        agents[agentIndex].rotation = Quaternion.RotateTowards(
            agents[agentIndex].rotation,
            targetRot,
            turnSpeedDegPerSec * Time.deltaTime
        );
    }

    void SteerTowardsDesiredVelocity(int agentIndex)
    {
        if (!agents[agentIndex]) return;
        // Debug.Log($"Mover t: {transform.gameObject.name}");

        Vector3 velocity = velocities[agentIndex];
        // Vector3 desiredVelocity = ComputeDesiredVelocityCircle(agentIndex);
        
        Vector3 desiredVelocity = meshMover.ComputeDesiredVelocityNormalized(agentIndex, agents[agentIndex]);
        // Vector3 desiredVelocity = ComputeDesiredVelocity();
        Vector3 steer = desiredVelocity - velocity;

        // Limit acceleration
        steer = Vector3.ClampMagnitude(steer, maxAccel);

        velocity += steer * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        agents[agentIndex].position += desiredVelocity * maxSpeed * Time.deltaTime;
        velocities[agentIndex] = desiredVelocity;
    }
    
    Vector3 ComputeDesiredVelocityCircle(int agentIndex)
    {
        Vector3 toCenter = agents[agentIndex].position - orbitCenter.position;

        float dist = toCenter.magnitude;
        if (dist < 0.001f)
            return agents[agentIndex].forward * orbitSpeed;

        Vector3 radialDir = toCenter / dist;

        // Tangent direction (left-hand orbit)
        Vector3 tangent = Vector3.Cross(clockWise ? Vector3.up : Vector3.down, radialDir);

        // Maintain radius (push in or out)
        float radiusError = dist - orbitRadius;
        Vector3 correction = -radialDir * radiusError * radialCorrection;

        Vector3 desired = tangent * orbitSpeed + correction;
        desired.y = 0;

        return desired;
    }
    
    Vector3 ComputeSeparation(Transform me, IList<Transform> agents)
    {
        float r2 = separationRadius * separationRadius;
        Vector3 push = Vector3.zero;
        int count = 0;

        for (int i = 0; i < agents.Count; i++)
        {
            Transform other = agents[i];
            if (other == me) continue;

            Vector3 d = me.position - other.position;
            float dist2 = d.sqrMagnitude;
            if (dist2 <= 0f || dist2 > r2) continue;

            // Weight: stronger when closer. (dist2 in denom avoids sqrt)
            push += d / dist2;
            count++;
        }

        if (count == 0) return Vector3.zero;

        push /= count;
        return push.normalized * separationStrength;
    }
}
