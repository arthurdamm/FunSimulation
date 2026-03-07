using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private ImprovedBeam beamPrefab;

    [SerializeField] private float fireCooldown = 2f;
    
    private float lastFireTime = 0f;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log($"Start time: {Time.time}");
    }

    // Update is called once per frame
    void Update()
    {
        if (target && Time.time - lastFireTime >= fireCooldown)
        {
            Debug.Log($"Firing! {Time.time}");
            Instantiate(beamPrefab).Fire(transform, target);
            lastFireTime = Time.time;
        }
    }
}
