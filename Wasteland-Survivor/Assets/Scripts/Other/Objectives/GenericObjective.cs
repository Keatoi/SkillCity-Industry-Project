using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericObjective : Objective
{
    public GenericObjective(string title, string description)
    {
        Title = title;
        Description = description;
        IsCompleted = false;
    }
    public override void CompleteObjective()
    {
        IsCompleted = true;
    }
}
