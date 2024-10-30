using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : InteractableObject
{
    public DayNightCycle DayNightCycle;
    [SerializeField] int RestAmount = 4;
    // Start is called before the first frame update

    public override void InteractAction(Collider Player)
    {
        DayNightCycle.IncreaseHours(RestAmount);
    }
}
