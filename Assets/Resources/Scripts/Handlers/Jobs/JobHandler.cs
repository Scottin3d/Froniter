using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WorkPlaceType {
    Default,
    Storage,
}


public class Job {
    public Job(JobObject mJobObj, WorkPlace mWorkPlace, Agent mAgent = null) {
        JobObject = mJobObj;
        JobAgent = mAgent;
        JobWorkPlace = mWorkPlace;
    }

    public Job(Job mJob) {
        JobObject = mJob.JobObject;
        JobAgent = mJob.JobAgent;
        JobWorkPlace = mJob.JobWorkPlace;
    }

    public JobObject JobObject { get { return jobObject; } private set { jobObject = value; } }
    private JobObject jobObject = null;
    public Agent JobAgent { get { return jobAgent; } private set { jobAgent = value; } }
    private Agent jobAgent = null;

    public WorkPlace JobWorkPlace{ get { return jobWorkPlace; } private set { jobWorkPlace = value; } }
    private WorkPlace jobWorkPlace = null;

    public void SetAgent(Agent mAgent) {
        JobAgent = mAgent;
    }

    public bool HasAgent() {
        return jobAgent != null;
    }
}


public class JobHandler : MonoBehaviour {
    // instance
    public static JobHandler current;
    // containers
    [SerializeField] Transform inGameWorkPlaces = null;
    Dictionary<string, List<WorkPlace>> workPlaces = new Dictionary<string, List<WorkPlace>>();

    //jobs
    public List<JobObject> availJobsTest;
    Dictionary<string, List<Job>> jobs = new Dictionary<string, List<Job>>();
    Dictionary<string, Queue<Job>> availableJobs = new Dictionary<string, Queue<Job>>();


    [TextArea(10, 15)]
    public string jobstest;
    void Awake() {
        current = this;
        Debug.Assert(inGameWorkPlaces != null, "Please assign workplaces in editor!");

        // initialize work places
        InitInGameWorkPlaces();

    }

    private void Update() {
        availJobsTest = new List<JobObject>();
        foreach (KeyValuePair<string, List<Job>> job in jobs) {
            foreach (Job j in job.Value) {
                availJobsTest.Add(j.JobObject);
            }
            break;
        }
    }

    public void CreateJob(string mJobID, Job mJob) {
        if (!jobs.ContainsKey(mJobID)) {
            jobs.Add(mJobID, new List<Job>());
        }
        if (!availableJobs.ContainsKey(mJobID)) {
            availableJobs.Add(mJobID, new Queue<Job>());
        }
        jobs[mJobID].Add(mJob);
        availableJobs[mJobID].Enqueue(mJob);

    }

    public Job GetAvailableJob(string mJobKey) {
        // TODO assign professons
        Job retJob = null;
        if (availableJobs.ContainsKey(mJobKey)) {
            
            retJob = availableJobs[mJobKey].Dequeue();
            if (availableJobs[mJobKey].Count == 0) {
                availableJobs.Remove(mJobKey);
            }
        }

        return retJob;
    }


    private void InitInGameWorkPlaces() {
        foreach (Transform child in inGameWorkPlaces) {
            WorkPlace childWorkPlace = child.GetComponent<WorkPlace>();
            if (workPlaces.ContainsKey(childWorkPlace.workObject.workID)) {
                workPlaces[childWorkPlace.workObject.workID].Add(childWorkPlace);
            } else {
                workPlaces.Add(childWorkPlace.workObject.workID, new List<WorkPlace>());
                workPlaces[childWorkPlace.workObject.workID].Add(childWorkPlace);
            }

            
        }
    }

    public Transform GetTargetTransform(string mType) {
        // check diction for type
        // return first
        if (workPlaces.ContainsKey(mType)) {
            return workPlaces[mType][0].workObject.GetAccessPoint();
        }

        return null;
    }
}
