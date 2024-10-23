using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Objective : MonoBehaviour
{
    public string Title { get; protected set; }
    public string Description { get; protected set; }
    public bool IsCompleted {  get; protected set; }
    //public Objective(string title, string description)
    //{
    //    Title = title;
    //    Description = description;
    //    IsCompleted = false;
    //}
    public abstract void CompleteObjective();
    public virtual void ShowStatus()
    {
        Debug.Log($"{Title}: {(IsCompleted ? "Completed" : "Pending")}");
    }

}
