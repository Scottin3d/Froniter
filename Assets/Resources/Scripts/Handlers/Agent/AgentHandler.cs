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
    public AgentObject[] agentObjects = new AgentObject[2];

    private void Awake() {
        current = this;
        Debug.Assert(agentPrefab != null, "Please assign an agent prefab in the editor!");
        Debug.Assert(inGameAgents != null, "Please assign inGameAgents in the editor!");
    }

    private void Start() {
        InputHandler.current.OnPress_g += Input_HandleOnPress_g;
        InputHandler.current.OnPress_l += Input_HandleOnPress_l;

    }

    private void Input_HandleOnPress_g() {
        SpawnAgent(Instantiate(agentObjects[0]), JobType.gatherer);
    }

    private void Input_HandleOnPress_l() {
        SpawnAgent(Instantiate(agentObjects[0]), JobType.lumberjack);
    }

    private void SpawnAgent(AgentObject mAgentObject, JobType job) {
        GameObject agentClone = Instantiate<GameObject>(agentPrefab, inGameAgents);
        agentClone.name = "Agent" + gameAgents.Count + "." + job;
        Agent agent = agentClone.GetComponent<Agent>();
        agent.SetHome(transform);
        // TODO change hard coded
        //Job agentJob = JobHandler.current.GetAvailableJob(job);
        Job agentJob = null;

        // int object
        mAgentObject.InitObject();
        agent.InitializeAgent(mAgentObject, ref agentJob, job);

        // init class
        gameAgents.Add(agent);
        OnAgentCreation?.Invoke(agent);

    }
}
