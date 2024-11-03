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
    public bool Rescued = false;
    public float health = 100f;
    public ObjectiveManager OM;
    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    ///////////Npc changehealth funtion for NPC attack -> Ai ///////////////////////////////////////////////////////////////////////////////////////////
    public void changehealthNPC(float damage,NpcController attacker) 
    { 
        health -=damage;
        onhealthchange(attacker);
    }
    
    void onhealthchange(NpcController attacker) { 
        Debug.Log("Dealt damage"+attacker.name);
        if(health  <= 0&& this!=null) {

      attacker.enemmyspotted = false;
      attacker.findcamp = true;

        Destroy(gameObject);
        }
        else { return; }
    }

    ///////////Ai changehealth For AI Attack -> NPC///////////////////////////////////////////////////////////////////////////////////////////
    public void changehealthAi(float damage)
    {
        health -= damage;
        onhealthchangeAi();
    }

    void onhealthchangeAi()
    {
        if (health <= 0 && this != null)
        {

            // attacker.playerspotted = false;
            // attacker.playerwasspotted = false;

            OM.KillCheck("Defend the camp");
            Destroy(gameObject);
        }
        else { return; }
    }
}
