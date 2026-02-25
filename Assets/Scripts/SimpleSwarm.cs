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

    public float beamCooldown = 1f;

    private Vector3[] velocities;
    private float[] previousSmells;
    private float[] lastBeamFiredTimes;

    private List<Transform> agents = new();
    private List<Transform> foods = new();
    

    void Start()
    {
        velocities = new Vector3[count];
        previousSmells = new float[count];
        lastBeamFiredTimes = new float[count];

        Debug.Log($"Start Time: {Time.time}");
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = Random.insideUnitSphere * areaSize;
            pos.y = 0.5f;

            GameObject go = Instantiate(shipPrefab, pos, Quaternion.identity);
            go.transform.parent = transform;
            SteeringMover sm = go.GetComponent<SteeringMover>();
            sm.orbitCenter = orbitCenter;
            agents.Add(go.transform);
            
            velocities[i] = Random.onUnitSphere;
            velocities[i].y = 0f;
            lastBeamFiredTimes[i] = Time.time;
        }
    }

    void Update()
    {
        for (int i = 0; i < agents.Count; i++)
        {
            Transform t = agents[i];
            Debug.Log($"t: {t.gameObject.name}");
            
            t.position += velocities[i] * (speed * Time.deltaTime);
            
            if (Time.deltaTime % i == 0) TryToFire(t, i);
        }
    }

    private void TryToFire(Transform source, int agentIndex)
    {
        if (foods.Count > 0 && Time.time - lastBeamFiredTimes[agentIndex] >= beamCooldown)
        {
            Debug.Log("FIRING!");
            lastBeamFiredTimes[agentIndex] = Time.time;
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
