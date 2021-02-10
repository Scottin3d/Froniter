using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public WorkPlace JobWorkPlace { get { return jobWorkPlace; } private set { jobWorkPlace = value; } }
    private WorkPlace jobWorkPlace = null;

    public void SetAgent(Agent mAgent) {
        JobAgent = mAgent;
    }

    public bool HasAgent() {
        return jobAgent != null;
    }
}
