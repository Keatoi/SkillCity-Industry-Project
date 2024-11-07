using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class AnimalBehaviour : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 5f;
    public float speed;
    private NavMeshAgent navMeshAgent;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if(distance < detectionRange)
        {
            //get direction opposite to player
            Vector3 oppositeDir = (transform.position - player.position).normalized;
            //Turn animal away from player
            Quaternion targetRot = Quaternion.LookRotation(oppositeDir);
            Vector3 targetPos = transform.position + oppositeDir * detectionRange;

            navMeshAgent.SetDestination(targetPos);
            navMeshAgent.speed = speed;
        }
        else
        {
            navMeshAgent.ResetPath();
        }
    }
}
