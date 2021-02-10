using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WorkPlaceType {
    Default,
    Storage,
}

public class JobHandler : MonoBehaviour {
    // instance
    public static JobHandler current;
    // containers
    [SerializeField] Transform inGameWorkPlaces = null;
    Dictionary<JobType, List<WorkPlace>> workPlaces = new Dictionary<JobType, List<WorkPlace>>();

    //jobs
    public List<JobObject> availJobsTest;
    Dictionary<JobType, List<Job>> jobs = new Dictionary<JobType, List<Job>>();
    Dictionary<JobType, Queue<Job>> availableJobs = new Dictionary<JobType, Queue<Job>>();


    [TextArea(10, 15)]
    public string jobstest;
    void Awake() {
        current = this;
        Debug.Assert(inGameWorkPlaces != null, "Please assign workplaces in editor!");

        // initialize work places
        InitInGameWorkPlaces();

    }

    private void Update() {
        /*
        availJobsTest = new List<JobObject>();
        foreach (KeyValuePair<string, List<Job>> job in jobs) {
            foreach (Job j in job.Value) {
                availJobsTest.Add(j.JobObject);
            }
            break;
        }
        */
    }

    public void CreateJob(JobType mJobID, Job mJob) {
        if (!jobs.ContainsKey(mJobID)) {
            jobs.Add(mJobID, new List<Job>());
        }
        if (!availableJobs.ContainsKey(mJobID)) {
            availableJobs.Add(mJobID, new Queue<Job>());
        }
        jobs[mJobID].Add(mJob);
        availableJobs[mJobID].Enqueue(mJob);

    }

    public Job GetAvailableJob(JobType mJobKey) {
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
            if (workPlaces.ContainsKey(childWorkPlace.workObject.workJobType)) {
                workPlaces[childWorkPlace.workObject.workJobType].Add(childWorkPlace);
            } else {
                workPlaces.Add(childWorkPlace.workObject.workJobType, new List<WorkPlace>());
                workPlaces[childWorkPlace.workObject.workJobType].Add(childWorkPlace);
            }

            
        }
    }

    public Transform GetTargetTransform(JobType mType) {
        // check diction for type
        // return first
        if (workPlaces.ContainsKey(mType)) {
            return workPlaces[mType][0].workObject.GetAccessPoint();
        }

        return null;
    }
}
