using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class AiRef : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    [Header("STATS")]
    public float updatepathdelay = 0.2f;
    public float attackdelay = 1.0f;
    public bool playerspotted= false;
    public bool playerwasspotted= false;
    public bool following = false;  
    public float health = 100f;

    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void changehealth(float damage) { 
    health -=damage;
        onhealthchange();
    }
    
    void onhealthchange() { 
     if(health  <= 0) {
        Destroy(gameObject);
        }
    }
}
