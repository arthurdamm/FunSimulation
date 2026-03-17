using System.Text;
using UnityEngine;

public class MeshFormation : MonoBehaviour
{
    /* Want to access the vertices of a mesh */
    [SerializeField] public GameObject gameObj;
    [SerializeField] public GameObject shipPrefab;

    public MeshFilter meshFilter;
    public Mesh mesh;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshFilter = gameObj.GetComponent<MeshFilter>();
        mesh = meshFilter.sharedMesh;
        Bounds bounds = mesh.bounds;
        Debug.Log($"bounds.center: {bounds.center}, {bounds.extents}, {bounds.extents}");
        LogFields(new { bounds.center, bounds.extents, bounds.size });
        var sb = new StringBuilder("Vertices: ");
        LogFields(new { gameObj.transform.localScale });

        transform.position = gameObj.transform.position;
        foreach (var v in mesh.vertices)
        {
            sb.Append(v);
            sb.Append("; ");
            v.Scale(gameObj.transform.localScale);
            sb.Append(transform.TransformPoint(v));
            
            Vector3 pos = transform.TransformPoint(v);
            // pos.Scale(gameObj.transform.localScale);
            // Instantiate(shipPrefab, pos,
            //     Quaternion.LookRotation(gameObj.transform.forward, Vector3.up));
            sb.Append(", ");
        }
        Debug.Log(sb);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void LogFields(object obj)
    {
        var sb = new StringBuilder();
        // sb.Append(obj.ToString());
        // sb.Append("; ");
        // sb.Append($"fullname: {obj.GetType().FullName}, ");
        foreach (var prop in obj.GetType().GetProperties())
        {
            sb.Append($"{prop.Name}: {prop.GetValue(obj)}, ");
        }
        Debug.Log(sb);
    }
    


}
