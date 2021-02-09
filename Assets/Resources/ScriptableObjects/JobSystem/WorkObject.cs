using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New Workplace Object", menuName = "Job System/Create Workplace")]
public class WorkObject : ScriptableObject {
    public string workID;
    public int maxJobs;
    public ResourceType[] workResourceTypes;

    private Transform workLoc = null;
    private GameObject[] workAccessPoints;

    private bool init = false;
    public void InitWorkObject(Transform mLoc, GameObject[] mAccessPoints) {
        if (!init) {
            workLoc = mLoc;
            workAccessPoints = mAccessPoints;

            init = true;
        }
    }

    public Transform GetAccessPoint() {
        return workAccessPoints[0].transform;
    }


}
