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

   
    public string jobID;
    public ResourceType[] jobResources;
    public WorkPlace jobWorkplace;
    [TextArea(10, 15)]
    public string jobDescription;
    public Sprite jobUIicon;
    public Color jobUIiconColor = Color.white;

    public JobObject(JobObject mJobObj) {
        this.jobID = mJobObj.jobID;
        this.jobResources = mJobObj.jobResources;
        this.jobWorkplace = mJobObj.jobWorkplace;
        this.jobDescription = mJobObj.jobDescription;
        this.jobUIicon = mJobObj.jobUIicon;
        this.jobUIiconColor = mJobObj.jobUIiconColor;
    }
}
