using System;
using System.Collections.Generic;
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
    [SerializeField] private float cellSize = 10.0f;
    [SerializeField] private float separationWeight = 0.5f;
    [SerializeField] private float seperationRadius = 10.0f;
    private float _separationRadiusSqr;

    private Bounds _agentBounds;
    
    private Transform[] _agentTransforms;
    private Vector3[] _agentVelocities;

    private SpatialHashGrid3D _hashGrid;
    
    private void Start()
    {
        RecaculateDerivedValues();
        
        _agentTransforms = new Transform[AgentCount];
        _agentVelocities = new Vector3[AgentCount];
        
        _agentBounds = agentPrefab.GetComponent<MeshFilter>().sharedMesh.bounds;

        _hashGrid = new SpatialHashGrid3D(cellSize);
        
        // SpawnRow();
        SpawnShere();
        InitialVelocities();

    }



    private void OnValidate()
    {
        RecaculateDerivedValues();
    }

    private void RecaculateDerivedValues()
    {
        _separationRadiusSqr = seperationRadius * seperationRadius;
        Debug.Log($"Sep is now: {_separationRadiusSqr}");
    }

    private void Update()
    {
        for (int agentIndex = 0; agentIndex < _agentVelocities.Length; agentIndex++)
        {
            _hashGrid.Insert(agentIndex, _agentTransforms[agentIndex].position);
            
            ComputeOrbitalVelocity(agentIndex);
            Vector3 separation = ComputeAgentSeperation(agentIndex);
            _agentVelocities[agentIndex] += separationWeight * separation; 
            FaceVelocity(agentIndex);
            _agentTransforms[agentIndex].position += agentSpeed * Time.deltaTime * _agentVelocities[agentIndex];
        }
        
        _hashGrid.Clear();
    }

    private Vector3 ComputeAgentSeperation(int agentIndex)
    {
        Vector3 separation = new();
        
        List<int> neighbors = new();
        _hashGrid.ForEachNeighbor(_agentTransforms[agentIndex].position, neighbors.Add);
        foreach (int neighborIndex in neighbors)
        {
            Vector3 away = _agentTransforms[agentIndex].position - _agentTransforms[neighborIndex].position;
            float sqrDistance = away.sqrMagnitude;
            if (sqrDistance < 1e-6 || sqrDistance >= _separationRadiusSqr)
            {
                continue;
            }

            float distance = Mathf.Sqrt(sqrDistance);
            Vector3 awayNormalized = away / distance;
            float separationStrength = 1f - (distance / seperationRadius);
            separation += awayNormalized * separationStrength;
        }

        return separation;
    }

    private void SpawnShere()
    {
        for (int agentIndex = 0; agentIndex < AgentCount; agentIndex++)
        {
            Vector3 spawnPosition = Random.insideUnitSphere * spawnAreaSize;
            GameObject agentGo = Instantiate(agentPrefab, spawnPosition, Quaternion.identity);
            _agentTransforms[agentIndex] = agentGo.transform;    
        }
    }
    
    private void SpawnRow()
    {
        Vector3 spawnPosition = transform.position;
        Vector3 shiftRight = Vector3.zero;
        Vector3 shiftBack = Vector3.zero;
        for (int agentIndex = 0; agentIndex < AgentCount; agentIndex++)
        {
            GameObject agentGo = Instantiate(agentPrefab, spawnPosition + shiftRight + shiftBack, Quaternion.identity);
            _agentTransforms[agentIndex] = agentGo.transform;

            shiftRight += Vector3.right * (_agentBounds.size.x + spawnPadding);
            if ((agentIndex + 1) % agentsPerRow == 0)
            {
                shiftBack += Vector3.back * (_agentBounds.size.z + spawnPadding);
                shiftRight = Vector3.zero;
            }
        }
    }
    
    private void InitialVelocities()
    {
        for(int agentIndex = 0; agentIndex < _agentVelocities.Length; agentIndex++)
        {
            _agentVelocities[agentIndex] = Random.onUnitSphere;
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