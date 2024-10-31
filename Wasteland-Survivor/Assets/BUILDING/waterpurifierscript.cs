using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterpurifierscript : InteractableObject
{
    [SerializeField] bool ison = false;
    [SerializeField] Transform effects;
    [SerializeField] ResourceSystem playerinv;

    public void Start()
    {
       playerinv = GameObject.FindGameObjectWithTag("Player").GetComponent<ResourceSystem>();

    }
    public override void InteractAction(Collider Player)
    {
        if (playerinv.Waterchip >0)
        {
              ison = !ison;
            effects.gameObject.SetActive(ison);
            playerinv.Waterchip = 0;
        }
      
    }
}
