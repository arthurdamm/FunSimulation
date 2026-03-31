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

    [Header("Agent Stats")]
    
    [SerializeField] private float agentSpeed = 5f;

    [SerializeField] private Transform orbitCenter;

    private Bounds agentBounds;
    
    private Transform[] _agentTransforms;
    private Vector3[] _agentVelocities;
    
    private void Start()
    {
        _agentTransforms = new Transform[AgentCount];
        _agentVelocities = new Vector3[AgentCount];

        agentBounds = agentPrefab.GetComponent<MeshFilter>().sharedMesh.bounds;
        
        SpawnRow();
        InitialVelocities();

    }

    private void Update()
    {
        for (int i = 0; i < _agentVelocities.Length; i++)
        {
            ComputeOrbitalVelocity(i);
            _agentTransforms[i].position += agentSpeed * Time.deltaTime * _agentVelocities[i];
            FaceVelocity(i);
        }
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
    
    private void InitialVelocities()
    {
        for(int i = 0; i < _agentVelocities.Length; i++)
        {
            _agentVelocities[i] = Random.onUnitSphere;
        }
    }

    private void FaceVelocity(int agentIndex)
    {
        float degreesPerSecond = 10f;
        /*
         Rotate forward vector to velocity vector
         
        lookRotation = Q.LookRotation(vel[i])
        t.rot = slerp(t.orient, lookRot, dt)
         */
        
        Quaternion lookRotation = Quaternion.LookRotation(_agentVelocities[agentIndex], Vector3.up);
        _agentTransforms[agentIndex].rotation =
            Quaternion.Slerp(_agentTransforms[agentIndex].rotation, lookRotation, Time.deltaTime);
    }

    private void ComputeOrbitalVelocity(int agentIndex)
    {
        Vector3 forward = Vector3.Cross(orbitCenter.position - _agentTransforms[agentIndex].position, _agentTransforms[agentIndex].up);
        _agentVelocities[agentIndex] = forward.normalized;
    }
}
}