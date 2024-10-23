using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillObjective : Objective
{
   public int ReqKills { get; private set; }
   public int currentKills;
    
    public KillObjective(string title, string description, int reqKills)
    {
        Title = title;
        Description = description;
        ReqKills = reqKills;
        currentKills = 0;
        IsCompleted = false;
    }
    public override void CompleteObjective()
    {
        IsCompleted = true;
        Debug.Log($"Kill Objective Completed: {Title}");
    }
    public void IncrementKills()
    {
        if (IsCompleted) { return; }
        currentKills++;
        Debug.Log($"Current Kills: {currentKills}/{ReqKills}");
        if (currentKills >= ReqKills)
        {
            CompleteObjective();
        }
    }
}
