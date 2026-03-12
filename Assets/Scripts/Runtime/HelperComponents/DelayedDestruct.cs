using UnityEngine;

public class DelayedDestruct : MonoBehaviour
{
    [SerializeField] private float delay = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, delay);
    }

}
