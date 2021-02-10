using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class WorkPlace : MonoBehaviour {

    //public WorkObject WorkObject { get { return workObject; } private set { workObject = value; } }
    public WorkObject workObject = null;
    public GameObject prefab = null;
    public Transform PrefabAccessPoints = null;

    public JobObject jobObject;
    public Inventory workplaceInventory;
    [SerializeField] private Job[] workJobs;
    [SerializeField] private int maxNumberOfJobs;
    [SerializeField] private int openJobs;

    // on awake
    // set in object from prefab
    //   workLoc = prefab.transform
    //   accesspoints[] foreach a in AccessPoints 
    private void Start() {
        Debug.Assert(workObject != null, "Please set workObject for " + transform.name);
        //Debug.Assert(prefab != null, "Please set prefab for " + transform.name);
        Debug.Assert(PrefabAccessPoints != null, "Please set PrefabAccessPoint object for " + transform.name);

        // int workplace object
        workObject = Instantiate<WorkObject>(workObject);

        // init jobs
        maxNumberOfJobs = workObject.maxJobs;
        workJobs = new Job[maxNumberOfJobs];

        for (int i = 0; i < maxNumberOfJobs; i++) {
            workJobs[i] = new Job(Instantiate<JobObject>(jobObject, transform), this);
            JobHandler.current.CreateJob(jobObject.jobID, workJobs[i]);
        }

        // init open jobs
        openJobs = maxNumberOfJobs;

        // init access points
        GameObject[] points = new GameObject[maxNumberOfJobs];
        for (int i = 0; i < PrefabAccessPoints.childCount; i++) {
            points[i] = PrefabAccessPoints.GetChild(i).gameObject;
        }

        workObject.InitWorkObject(transform, points);

        // init inventory
        workplaceInventory = new Inventory(this);
        InventoryHandler.current.AddWorkplaceInventory(this);

    }

}


