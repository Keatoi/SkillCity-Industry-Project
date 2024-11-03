using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] public bool triggerActive = false;
    public Collider playercollider;
    public GameObject interacttext;
    // Update is called once per frame
    void Update()
    {
        if (triggerActive && Input.GetKeyDown(KeyCode.E))
        {
            InteractAction(playercollider);
        }
    }
    public  bool GetSetBool()
    {
        return triggerActive;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerActive = true;
            playercollider = other;
            interacttext = GameObject.FindGameObjectWithTag("itemtext");
            interacttext.GetComponent<TextMeshProUGUI>().text = ("Press E to Interact"); 
            interacttext.SetActive(true);
            if(this.gameObject.TryGetComponent<pickup>(out pickup pickupref )) {
                pickupref.Showtext();
            }

        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerActive = false;
            playercollider= null;
            interacttext.GetComponent<TextMeshProUGUI>().text = "";
           // interacttext.SetActive(false);
        }
    }
    public virtual void InteractAction(Collider playercollider)
    {
        
    }
    public void Changetext(string text)
    {
        interacttext.GetComponent<TextMeshProUGUI>().text = text;
    }
  
}
