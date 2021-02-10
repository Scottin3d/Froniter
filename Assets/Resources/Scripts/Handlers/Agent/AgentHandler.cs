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
        if (Input.GetKeyDown(KeyCode.L)) {
            SpawnAgent(JobType.lumberjack);
        }

        if (Input.GetKeyDown(KeyCode.G)) {
            SpawnAgent(JobType.gatherer);
        }
    }

    private void SpawnAgent(JobType job) {
        GameObject agentClone = Instantiate<GameObject>(agentPrefab, inGameAgents);
        agentClone.name = "Agent" + gameAgents.Count + "." + job;
        Agent agent = agentClone.GetComponent<Agent>();
        // TODO change hard coded
        //Job agentJob = JobHandler.current.GetAvailableJob(job);
        Job agentJob = null;
        agent.InitializeAgent(ref agentJob, job);
        gameAgents.Add(agent);
        OnAgentCreation?.Invoke(agent);

    }
}
