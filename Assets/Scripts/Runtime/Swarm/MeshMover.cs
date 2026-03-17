using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

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
        // Debug.Log($"ComputeDesiredVelocityNormalized: {agentIndex}: {vertex}");
        Vector3 direction = vertex - transform.position; // displacement vector
        if (direction.sqrMagnitude < 1e-6)
        {
            return Vector3.zero;
        }
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
            // Debug.Log($"GetPositions: {i}: {vertex}");
            // Vector3 direction = vertex - transform.position; // displacement vector
            positions.Add(vertex);
        }

        return positions;

    }

}
