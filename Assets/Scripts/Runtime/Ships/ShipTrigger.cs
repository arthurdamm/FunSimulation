using System;
using UnityEngine;

public class ShipTrigger : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;

    public event Action<int> handleShipDeath;
    private int agentId { set; get; } = -1;
    
    public int triggerCount = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (++triggerCount > 1)
        {
            // Debug.Log($"REPEAT OnTrigger {gameObject.name} {transform.position}");            
        }
        else
        {
            // Debug.Log($"OnTrigger {gameObject.name} {transform.position}");
            Instantiate(explosionPrefab, transform.position, transform.rotation);
            handleShipDeath?.Invoke(agentId);
            // Destroy(gameObject);            
        }
    }
    
    private void OnDestroy()
    {
        Debug.Log($"OnDestroy {gameObject.name}");
    }
}
