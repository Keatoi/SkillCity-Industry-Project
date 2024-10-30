using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : InteractableObject
{
    [SerializeField] bool ison= false;
    [SerializeField] Transform effects;

    public override void InteractAction(Collider Player)
    {
        ison = !ison;
        effects.gameObject.SetActive(ison);
    }

}
