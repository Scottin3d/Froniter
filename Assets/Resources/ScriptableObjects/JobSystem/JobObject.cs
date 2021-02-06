using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JobType {
    Default,
    Lumberjack,
}

[CreateAssetMenu(fileName = "New Job Object", menuName = "Inventory System/")]
public class JobObject : ScriptableObject
{
    public JobType jobType;
    public WorkPlaceType workPlaceType;
    [TextArea(10, 15)]
    public string jobDescription;
    public Sprite jobUIicon;
    public Color jobUIiconColor = Color.white;
}
