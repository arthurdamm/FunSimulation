using UnityEngine;

public class BoundsTester : MonoBehaviour
{
    [SerializeField] public Vector3 _size;
    private void OnDrawGizmos()
    {
        Vector3 size = _size;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * size.z / 2);
        Gizmos.DrawLine(transform.position, transform.position - transform.forward * size.z / 2);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * size.y / 2);
        Gizmos.DrawLine(transform.position, transform.position - transform.up * size.y / 2);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * size.x / 2);
        Gizmos.DrawLine(transform.position, transform.position - transform.right * size.x / 2);
    }
}

