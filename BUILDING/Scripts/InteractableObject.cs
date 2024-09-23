using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private bool triggerActive = false;
   
    // Update is called once per frame
    void Update()
    {
        if (triggerActive && Input.GetKeyDown(KeyCode.E))
        {
            InteractAction();
            Debug.Log("interact");
        }
    }
    

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerActive = true;
            Debug.Log("entered");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerActive = false;
        }
    }
    public virtual void InteractAction()
    {
        
    }
}
