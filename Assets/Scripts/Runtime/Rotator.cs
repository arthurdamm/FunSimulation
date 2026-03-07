using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float degreesPerSecond = 10f;
    [SerializeField] private bool world = true;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (world)
        {
            TickAngleAxisWorld();
        }
        else
        {
            TickAngleAxisLocal();
        }
    }

    void TickAngleAxisWorld()
    {
        Quaternion rotation = Quaternion.AngleAxis(degreesPerSecond * Time.deltaTime, Vector3.up);
        transform.rotation = rotation * transform.rotation;
    }
    
    void TickAngleAxisLocal()
    {
        Quaternion rotation = Quaternion.AngleAxis(degreesPerSecond * Time.deltaTime, Vector3.up);
        transform.rotation = transform.rotation * rotation;
    }
}
