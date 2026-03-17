using System;
using UnityEngine;

/*
       This class needs to instantiate and control a swarm of agents (ships).

      "Properties":
      -------------
       int AgentCount
       float SpawnAreaSize
       GO AgentPrefab

      State:
      -------
       int _agentIdToIndex[AgentCount]
       int _agentIndexToId[AgentCount]

      stuct AgentRef{
       public Transform Transform;

      }

      private Vector3 _agentVelocities[AgentCount]
      private Vector3 _agentPositions[AgentCount]

      private AgentRef _agentRefs[AgentCount]

      private int _lastAgentIndex = AgentCount - 1;

      Using element number in swarm as id
      onSpawn:


       for i: 0 to AgentCount - 1:
           AgentIdToIndex = i
           AgentIndexToId = i

      RemoveAgentById(agentId):
       int agentIndex = agentIdToIndex[agentId]
       if (agentIndex >= _lastAgentIndex) return
       int lastAgentId = agentIdToIndex[_lastAgentIndex]

       var tempLastAgentRefs = _agentRefs[_lastAgentIndex]
       _agentRefs[_lastAgentIndex] = _agentRefs[agentIndex]
       _agentRefs[agentIndex] = tempLastAgentRefs;

       agentIdToIndex[agentId] = _lastAgentIndex;
       agentIdToIndex[lastAgentId] = agentIndex;

       var tempVelocity = _agentVelocities[_lastAgentIndex];
       _agentVelocities[_lastAgentIndex] = _agentVelocities[agentIndex];
       _agentVelocities[agentIndex] = tempVelocity;
     */

namespace FunSimulation.Runtime.Swarm
{
public class StructSwarm : MonoBehaviour {
}
}