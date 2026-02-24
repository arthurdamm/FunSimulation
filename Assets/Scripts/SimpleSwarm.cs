using System.Collections.Generic;
using UnityEngine;

public class SimpleSwarm : MonoBehaviour
{
    public GameObject shipPrefab;     // sphere prefab
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private Beam beamPrefab;
    [SerializeField] private Transform orbitCenter;
    
    [SerializeField] private float smellStrength = 1f;
    [SerializeField] private float epsilon = 1f;
    [SerializeField] private float maxTurnAngle = 45f;
    
    public int count = 50;
    public float areaSize = 10f;
    public float speed = 2f;
    public float turnSpeed = 2f;

    public float lastBeamFiredTime;
    public float beamCooldown = 1f;

    Vector3[] velocities;
    private float[] previousSmells;

    private List<Transform> foods = new();

    void Start()
    {
        velocities = new Vector3[count];
        previousSmells = new float[count];
        lastBeamFiredTime = Time.time;

        Debug.Log($"Start Time: {lastBeamFiredTime}");
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = Random.insideUnitSphere * areaSize;
            pos.y = 0.5f;

            GameObject go = Instantiate(shipPrefab, pos, Quaternion.identity);
            go.transform.parent = transform;
            SteeringMover sm = go.GetComponent<SteeringMover>();
            sm.orbitCenter = orbitCenter;
            
            

            velocities[i] = Random.onUnitSphere;
            velocities[i].y = 0f;
        }
    }

    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform t = transform.GetChild(i);

            // gentle random steering
             // Vector3 steer = Random.insideUnitSphere;
             // steer.y = 0f;
             //
             // velocities[i] = Vector3.Lerp(
             //     velocities[i],
             //     (velocities[i] + steer).normalized,
             //     turnSpeed * Time.deltaTime
             // );

            float smell = GetFoodSmellStrength(t.position);
            if (smell > previousSmells[i])
            {
                // pass?
            }
            else
            {
                float turnAngle = Random.Range(-maxTurnAngle, maxTurnAngle);
                velocities[i] = Quaternion.AngleAxis(turnAngle, Vector3.up) * velocities[i];
            }
            
            previousSmells[i] = smell;
            t.position += velocities[i] * (speed * Time.deltaTime);

            // soft boundary push
            if (t.position.magnitude > areaSize)
            {
                velocities[i] = (-t.position).normalized;
                velocities[i].y = 0;
            }

            TryToFire(t);
        }
    }

    private void TryToFire(Transform source)
    {
        if (foods.Count > 0 && Time.time - lastBeamFiredTime >= beamCooldown)
        {
            Debug.Log("FIRING!");
            lastBeamFiredTime = Time.time;
            Beam beam = Instantiate(beamPrefab);
            beam.Fire(source, foods[Random.Range(0, foods.Count)]);
        }        
    }
    
    public void TrySpawnFood(Vector3 pos)
    {
        GameObject food = Instantiate(foodPrefab, pos, Quaternion.identity);
        foods.Add(food.transform);
        orbitCenter = food.transform;
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
}
