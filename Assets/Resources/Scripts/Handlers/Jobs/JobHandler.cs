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
    [SerializeField] public Object[] jobs;
    [SerializeField] Transform inGameWorkPlaces = null;
    Dictionary<WorkPlaceType, List<WorkPlace>> workPlaces = new Dictionary<WorkPlaceType, List<WorkPlace>>();
    void Awake() {
        current = this;
        Debug.Assert(inGameWorkPlaces != null, "Please assign workplaces in editor!");

        LoadJobs();
        // initialize work places
        InitInGameWorkPlaces();

    }

    private void InitInGameWorkPlaces() {
        foreach (Transform child in inGameWorkPlaces) {
            WorkPlace childWorkPlace = child.GetComponent<WorkPlace>();
            if (workPlaces.ContainsKey(childWorkPlace.workPlaceType)) {
                workPlaces[childWorkPlace.workPlaceType].Add(childWorkPlace);
            } else {
                workPlaces.Add(childWorkPlace.workPlaceType, new List<WorkPlace>());
                workPlaces[childWorkPlace.workPlaceType].Add(childWorkPlace);
            }

            
        }
    }

    private void LoadJobs() {
        jobs = Resources.LoadAll("ScriptableObjects/JobSystem", typeof(JobObject));
    }

    public Transform GetTargetTransform(WorkPlaceType mType) {
        // check diction for type
        // return first
        if (workPlaces.ContainsKey(mType)) {
            return workPlaces[mType][0].targetLocation;
        }

        return null;
    }
}
