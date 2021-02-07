using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JobType {
    Default,
    Lumberjack,
    Gatherer,
    Stonecutter

}

[CreateAssetMenu(fileName = "New Job Object", menuName = "Job System/Create Job")]
public class JobObject : ScriptableObject
{
    public JobType jobType;
    public ResourceType[] jobResources;
    public WorkPlaceType jobWorkplace;
    [TextArea(10, 15)]
    public string jobDescription;
    public Sprite jobUIicon;
    public Color jobUIiconColor = Color.white;
}
