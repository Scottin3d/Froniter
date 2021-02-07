using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadJobsFromFile : MonoBehaviour
{
    public TextAsset jobsJsonFile;

    void Start() {
        Jobss employeesInJson = JsonUtility.FromJson<Jobss>(jobsJsonFile.text);

        foreach (Job employee in employeesInJson.jobs) {
            Debug.Log("Found job: " + employee.jobID + " " + employee.resourceType + " " + employee.workplaceType);
        }
    }
}

[System.Serializable]
public class Job {
    public string jobID;
    public ResourceType resourceType;
    public WorkPlaceType workplaceType;
}

[System.Serializable]
public class Jobss {
    public Job[] jobs;
}
