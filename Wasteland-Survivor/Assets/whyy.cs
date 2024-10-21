using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class whyy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        
     
    }
    public void OnTriggerStay(Collider other)
    {
        Debug.Log(other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            // If we hit the player directly, the enemy can see the player

            Debug.Log("THIGN INSIDE THE BUMMMMABA");

        }

        if (other.CompareTag("NPC"))
        {
            Debug.Log("NPC INSDE THE BYMBSIDON");
        }
    }


    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("LEFT THE BYBFUABHYDF");
        }

    }

}
