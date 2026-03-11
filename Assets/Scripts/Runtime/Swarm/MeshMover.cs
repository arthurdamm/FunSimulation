using UnityEngine;

public class MeshMover
{
    public GameObject gameObj { set; get; }
    private MeshFilter meshFilter;
    private Mesh mesh;

    public void OnStart()
    {
        meshFilter = gameObj.GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        Debug.Log($"Vertex count: {mesh.vertices.Length }");
    }
    
    public Vector3 ComputeDesiredVelocityNormalized(int agentIndex, Transform transform)
    {
        Vector3 vertex = mesh.vertices[agentIndex];
        vertex.Scale(gameObj.transform.localScale); // changes vertices?
        vertex = gameObj.transform.TransformPoint(vertex); // actual point
        Vector3 direction = vertex - transform.position; // displacement vector
        direction.Normalize();
        return direction;

    }

}
