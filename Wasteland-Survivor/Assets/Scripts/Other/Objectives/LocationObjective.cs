using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationObjective : Objective
{
    public Vector3 TargetLoc { get; private set; }

    public LocationObjective(string title, string description,Vector3 targetLoc )
    {
        Title = title;
        Description = description;
        TargetLoc = targetLoc;
        IsCompleted = false;

    }
    public override void CompleteObjective()
    {
        IsCompleted = true;
        Debug.Log($"Location Objective Completed: {Title}");
    }
    public bool CheckDistance(Vector3 playerLoc)
    {
        return Vector3.Distance(playerLoc, TargetLoc) < 1f;
    }
}
