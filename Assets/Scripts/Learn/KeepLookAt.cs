using UnityEngine;

public class KeepLookAt : MonoBehaviour
{
    [SerializeField] private Transform target;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 relativeDirection = (target.position - transform.position).normalized;
        Debug.Log($"Normal len: {relativeDirection.magnitude}");
        transform.rotation = Quaternion.LookRotation(relativeDirection);
    }
}
