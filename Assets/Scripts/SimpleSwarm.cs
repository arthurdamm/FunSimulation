using UnityEngine;

public class SimpleSwarm : MonoBehaviour
{
    public GameObject prefab;     // sphere prefab
    public int count = 50;
    public float areaSize = 10f;
    public float speed = 2f;
    public float turnSpeed = 2f;

    Vector3[] velocities;

    void Start()
    {
        velocities = new Vector3[count];

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = Random.insideUnitSphere * areaSize;
            pos.y = 0.5f;

            GameObject go = Instantiate(prefab, pos, Quaternion.identity);
            go.transform.parent = transform;

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
            Vector3 steer = Random.insideUnitSphere;
            steer.y = 0f;

            velocities[i] = Vector3.Lerp(
                velocities[i],
                (velocities[i] + steer).normalized,
                turnSpeed * Time.deltaTime
            );

            t.position += velocities[i] * speed * Time.deltaTime;

            // soft boundary push
            if (t.position.magnitude > areaSize)
            {
                velocities[i] = (-t.position).normalized;
                velocities[i].y = 0;
            }
        }
    }
}
