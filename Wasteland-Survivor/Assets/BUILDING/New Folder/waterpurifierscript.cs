using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waterpurifierscript : InteractableObject
{
    [SerializeField] bool ison = false;
    [SerializeField] Transform effects;
    [SerializeField] ResourceSystem playerinv;
    public ObjectiveManager manager;
    

    public void Start()
    {
       playerinv = GameObject.FindGameObjectWithTag("Player").GetComponent<ResourceSystem>();
        Debug.Log("WATER");
        manager.CompleteObjective("Build Hydro-Purifier");
    }
    public override void InteractAction(Collider Player)
    {
        if (playerinv.Waterchip == true)
        {
              ison = !ison;
            effects.gameObject.SetActive(ison);
           
        }
      
    }
}
