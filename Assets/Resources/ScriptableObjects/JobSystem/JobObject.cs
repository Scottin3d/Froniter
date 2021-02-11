using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Job Object", menuName = "Job System/Create Job")]
public class JobObject : ScriptableObject
{

   
    public JobType ID;
    public ItemObject[] resources;
    private WorkPlace workplace;
    [TextArea(10, 15)]
    public string description;
    public Sprite UIicon;
    public Color UIiconColor = Color.white;

    public WorkPlace JobWorkplace { get => workplace; set => workplace = value; }

    public JobObject(JobObject mJobObj) {
        ID = mJobObj.ID;
        resources = mJobObj.resources;
        JobWorkplace = mJobObj.JobWorkplace;
        description = mJobObj.description;
        UIicon = mJobObj.UIicon;
        UIiconColor = mJobObj.UIiconColor;
    }

    public void SetWorkplace(WorkPlace mWorkplace) {
        JobWorkplace = mWorkplace;
    }


}
