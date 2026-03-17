using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FunSimulation.Runtime.Swarm
{
public class Swarm : MonoBehaviour
{
    [Header("Agent Spawn")]
    [SerializeField] public GameObject agentPrefab;

    [field: SerializeField] public int AgentCount { get; private set; } = 10;
    
    [SerializeField] private float spawnAreaSize;
    [SerializeField] private float spawnPadding = .01f;
    [SerializeField] private int agentsPerRow = 3;

    private Bounds agentBounds;
    
    private Transform[] _agentTransforms;
    private Vector3[] _agentVelocities;
    
    private void Start()
    {
        _agentTransforms = new Transform[AgentCount];
        _agentVelocities = new Vector3[AgentCount];

        agentBounds = agentPrefab.GetComponent<MeshFilter>().sharedMesh.bounds;
        
        SpawnRow();

    }

    private void SpawnRow()
    {
        Vector3 spawnPosition = transform.position;
        Vector3 shiftRight = Vector3.zero;
        Vector3 shiftBack = Vector3.zero;
        for (int i = 0; i < AgentCount; i++)
        {
            GameObject agentGo = Instantiate(agentPrefab, spawnPosition + shiftRight + shiftBack, Quaternion.identity);
            _agentTransforms[i] = agentGo.transform;

            shiftRight += Vector3.right * (agentBounds.size.x + spawnPadding);
            if ((i + 1) % agentsPerRow == 0)
            {
                shiftBack += Vector3.back * (agentBounds.size.z + spawnPadding);
                shiftRight = Vector3.zero;
            }
        }
    }
}
}