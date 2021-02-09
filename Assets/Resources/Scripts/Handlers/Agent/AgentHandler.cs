using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentHandler : MonoBehaviour
{
    // instance
    public static AgentHandler current;

    // event actions
    public event Action<Agent> OnAgentCreation;

    // containers
    public Transform inGameAgents = null;
    [SerializeField] GameObject agentPrefab = null;
    [SerializeField] List<Agent> gameAgents = new List<Agent>();


    private void Awake() {
        current = this;
        Debug.Assert(agentPrefab != null, "Please assign an agent prefab in the editor!");
        Debug.Assert(inGameAgents != null, "Please assign inGameAgents in the editor!");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            GameObject agentClone = Instantiate<GameObject>(agentPrefab, inGameAgents);
            Agent agent = agentClone.GetComponent<Agent>();
            Job agentJob = JobHandler.current.GetAvailableJob("lumberjack");
            agent.InitializeAgent(ref agentJob);
            gameAgents.Add(agent);
            OnAgentCreation?.Invoke(agent);
        }
    }
}
