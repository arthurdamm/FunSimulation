using Unity.Properties;
using UnityEngine;

public class OrbitalLook : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 10f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, rotation, Time.deltaTime);
        // transform.localRotation = rotation;
        transform.Translate(0,0,speed * Time.deltaTime);
        // Debug.Log($"Dist: {(target.position - transform.position).magnitude}");
        
    }
}
