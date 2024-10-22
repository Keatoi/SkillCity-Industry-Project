using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private bool triggerActive = false;
    private Collider playercollider;
    // Update is called once per frame
    void Update()
    {
        if (triggerActive && Input.GetKeyDown(KeyCode.E))
        {
            InteractAction(playercollider);
        }
    }
    

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerActive = true;
            playercollider = other;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerActive = false;
            playercollider= null;
        }
    }
    public virtual void InteractAction(Collider playercollider)
    {
        
    }
}
