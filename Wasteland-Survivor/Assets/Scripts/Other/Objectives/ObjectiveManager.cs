using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
   public List<Objective> _objectives = new List<Objective>();
    public static ObjectiveManager Instance; // Singleton instance

    private void Awake()
    {
        // Check if instance already exists
        if (Instance == null)
        {
            Instance = this; // Assign this instance
            DontDestroyOnLoad(gameObject); // Optional: Keep it across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate
        }
    }
    public void AddObjective(Objective obj)
    {
        //add new objective to List
        _objectives.Add(obj);
        DebugShowObjectives();
    }
    public void CompleteObjective(string title)
    {
        //Iterate through list, find matching objective that hasn't already been completed, mark as complete
        foreach (var objective in _objectives)
        {
            if (objective.Title == title && !objective.IsCompleted)
            {
                objective.CompleteObjective();
                Debug.Log($"Objective Completed: {title}");
                break;
            }
        }
    }
    public void CompleteLocationObjective(Vector3 playerPos)
    {
        foreach (var objective in _objectives)
        {
            if(objective is LocationObjective locationObjective)
            {
                if(locationObjective.CheckDistance(playerPos))
                {
                    locationObjective.CompleteObjective();
                }
            }
        }
    }
    public void DebugShowObjectives()
    {
        //iterate through list and show completion status of each objective
        foreach (var objective in _objectives)
        {
            objective.ShowStatus();
        }
    }
    public bool GetCompletionStatus(string title)
    {
        foreach(var objective in _objectives)
        {
            if (objective.Title == title && objective.IsCompleted) return true;
               
        }
        return false;
    }
    public void KillCheck(string title)
    {
        foreach (var objective in _objectives)
        {
            if (objective is KillObjective killObjective)
            {
                if (killObjective.Title == title)
                {
                    killObjective.IncrementKills();
                }
            }
        }
    }
    public bool IsObjective(string title)
    {
        foreach (var objective in _objectives)
        {
            if (objective.Title == title ) return true;

        }
        return false;
    }
}
