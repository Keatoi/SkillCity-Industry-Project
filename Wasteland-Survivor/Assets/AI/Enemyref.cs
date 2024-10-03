using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class Enemyref : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    [Header("STATS")]
    public float updatepathdelay = 0.2f;
    public float attackdelay = 1.0f;
    public AudioSource attackSound;
    public bool playerspotted= false;

    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
}
