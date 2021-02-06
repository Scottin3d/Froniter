using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum AgentState {
    Idle,
    Rest,
    MovingToGatherNode,
    Gathering,
    MovingToTarget,
}



public class AgentHandler : MonoBehaviour
{
    // instance
    public static AgentHandler current;

    // event actions
    public event Action<Agent> OnAgentCreation;

    // containers
    [SerializeField] GameObject agentPrefab = null;
    [SerializeField] List<Agent> gameAgents = new List<Agent>();

    private void Awake() {
        current = this;
        Debug.Assert(agentPrefab != null); ;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            GameObject agentClone = Instantiate<GameObject>(agentPrefab);
            Agent agent = agentClone.GetComponent<Agent>();
            agent.InitializeAgent(JobHandler.current.jobs[0] as JobObject);
            gameAgents.Add(agent);
            OnAgentCreation?.Invoke(agent);
        }
    }
}
