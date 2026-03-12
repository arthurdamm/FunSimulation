using System.Collections.Generic;
using UnityEngine;

public class MeshMover
{
    public GameObject gameObj { set; get; }
    private MeshFilter meshFilter;
    private Mesh mesh;

    public void OnEnable()
    {
        meshFilter = gameObj.GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        Debug.Log($"Vertex count: {mesh.vertices.Length }");
    }
    
    public Vector3 ComputeDesiredVelocityNormalized(int agentIndex, Transform transform)
    {
        Vector3 vertex = mesh.vertices[agentIndex];
        // vertex.Scale(gameObj.transform.localScale); // changes vertices?
        vertex = gameObj.transform.TransformPoint(vertex); // actual point
        Vector3 direction = vertex - transform.position; // displacement vector
        direction.Normalize();
        return direction;

    }
    
    public List<Vector3> GetPositions(int num)
    {
        List<Vector3> positions = new();
        for (int i = 0; i < num; i++)
        {
            Vector3 vertex = mesh.vertices[i];
            // vertex.Scale(gameObj.transform.localScale); // changes vertices?
            vertex = gameObj.transform.TransformPoint(vertex); // actual point
            // Vector3 direction = vertex - transform.position; // displacement vector
            positions.Add(vertex);
        }

        return positions;

    }

}
