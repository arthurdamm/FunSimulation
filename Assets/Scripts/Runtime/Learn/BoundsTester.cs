using System.Text;
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

    private void Start()
    {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;
        Bounds bounds = mesh.bounds;
        Debug.Log($"bounds.center: {bounds.center}, {bounds.extents}, {bounds.extents}");
        LogFields(new { bounds.center, bounds.extents, bounds.size });
    }
    
    void LogFields(object obj)
    {
        var sb = new StringBuilder();
        foreach (var prop in obj.GetType().GetProperties())
        {
            sb.Append($"{prop.Name}: {prop.GetValue(obj)}, ");
        }
        Debug.Log(sb);
    }
}

