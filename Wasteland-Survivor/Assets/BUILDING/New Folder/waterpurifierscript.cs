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
        manager = GameObject.Find("Director").GetComponent<ObjectiveManager>();
        Debug.Log("WATER");
        manager.CompleteObjective("Build Hydro-Purifier");
        Transform[] childTransforms = GetComponentsInChildren<Transform>();
        foreach (Transform trans in childTransforms)
        {Debug.Log(trans.name); 
            if (trans.gameObject.name == "effects")
            {
                effects = trans.gameObject.transform;
                effects.gameObject.SetActive(false);
            }
        }
    }
    public override void InteractAction(Collider Player)
    {
        playerinv = playercollider.GetComponent<ResourceSystem>();
        if (playerinv.Waterchip == true)
        {
              ison = !ison;
            effects.gameObject.SetActive(ison);
           
        }
      
    }
}
