using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WorkPlaceType { 
    Storage,
}

public class WorkHandler : MonoBehaviour {
    // instance
    public static WorkHandler current;

    // containers
    [SerializeField] Transform inGameWorkPlaces = null;
    Dictionary<WorkPlaceType, List<WorkPlace>> workPlaces = new Dictionary<WorkPlaceType, List<WorkPlace>>();
    void Awake() {
        current = this;
        Debug.Assert(inGameWorkPlaces != null, "Please assign workplaces in editor!");

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

    public Transform GetTargetTransform(WorkPlaceType mType) {
        // check diction for type
        // return first
        if (workPlaces.ContainsKey(mType)) {
            return workPlaces[mType][0].targetLocation;
        }

        return null;
    }
}
