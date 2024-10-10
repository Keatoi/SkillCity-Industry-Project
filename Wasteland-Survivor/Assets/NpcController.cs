using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class NpcController : MonoBehaviour
{
    public AiRef AiRef;
    private GameObject Player;
    private Transform player;
    private GameObject campcenter;
    private float PathUpdateDelay;
    private float attackdelay;
    private AudioSource audioSource;
    private Animator animator;

    //Bools
    public bool findcamp, patrol, waitingAtPoint;
    public float waitTimer; public float patrolWaitTime = 3f;public float stucktimer = 10f;
    Vector3 patrolDestination;

    public void Awake()
    {
        AiRef = GetComponent<AiRef>();
        Player = GameObject.FindGameObjectWithTag("Player");
        player = Player.GetComponent<Transform>();
        audioSource = AiRef.GetComponent<AudioSource>();
        animator = AiRef.GetComponent<Animator>();
        campcenter = GameObject.FindGameObjectWithTag("CampCenter");
        Debug.Log("AWAKED");

    }

    public void Update()
    {

        if (AiRef.following)
        {
            Following();
            AiRef.agent.stoppingDistance = 3f;

        }
        else if (findcamp)
        {
            Findcamp();
            if (Vector3.Distance(transform.position, campcenter.transform.position) <= 3f)
            {
                findcamp = false; patrol = true;
            }
        }
        else if (patrol)
        {
            AiRef.agent.stoppingDistance = 0;
            Patrol(); 
            if (waitingAtPoint)
            {
                Waitatpoint();
                Debug.Log("WAIT");
            }

        }
        
       
        if (AiRef.agent.velocity.magnitude <= 0.7f && !waitingAtPoint&& patrol )
        {
    
            Debug.Log("checking if stuck");
            stucktimer -= Time.deltaTime;
            if (stucktimer <= 0.1f)
            {
                if (AiRef.agent.velocity.magnitude <= 0.7f)
                {
                    patrolDestination = GetRandomPatrolPoint();
                    Debug.Log("Stuck, Changing point");
                }
                stucktimer = 10f;

            }

        }
        else { stucktimer = 10f; }
    }
    void Following()
    {
        if (Time.time >= PathUpdateDelay)
        {
            Debug.Log("Updating AI Path");
            PathUpdateDelay = Time.time + AiRef.updatepathdelay;
            AiRef.agent.SetDestination(player.position);
        }
    }
    void Findcamp()
    {
       
        if (campcenter != null)
        {
            if (Time.time >= PathUpdateDelay)
            {
                Debug.Log("Path Going To Camp Center");
                PathUpdateDelay = Time.time + AiRef.updatepathdelay;
                AiRef.agent.SetDestination(campcenter.transform.position);
            }

        }
    }
    Vector3 GetRandomPatrolPoint()
    {

        Vector3 randomDirection = Random.insideUnitSphere * 10;
        randomDirection += campcenter.transform.position; // Center around the patrol center
        NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, 240f, NavMesh.AllAreas);
        
        return navHit.position;
    }
    void Patrol()
    {Vector3 patrolDestination= GetRandomPatrolPoint();
        Vector3 Distance = patrolDestination - transform.position;
        //  Instantiate<GameObject>(square,patrolDestination,Quaternion.identity);  
        //  Debug.Log(Distance.magnitude.ToString()) ;
        //   Debug.Log("PAtrolling" + Vector3.Distance(patrolDestination, transform.position).ToString());

        if (!patrol) return; // Skip if we're not in patrolling mode
       
      
        if (Distance.magnitude <= 2f && !waitingAtPoint)
        {
            // Debug.Log("patrolling to location "+Distance.magnitude.ToString());
            AiRef.agent.SetDestination(patrolDestination);

            if (Distance.magnitude <= 1f)
            {
                Debug.Log("Close to point");
                waitingAtPoint = true; // Start waiting
              
                

            }

        }
    }
    void Waitatpoint()
    {
        Debug.Log("waiting");
        waitTimer -= Time.deltaTime;
        if (waitTimer <= 0)
        {
            Debug.Log("Moving to next point");
            patrolDestination = GetRandomPatrolPoint(); waitingAtPoint = false;
            waitTimer = 3f;

        }

    }


    //for UI
    public    void followingswicth()
    {
        AiRef.following = !AiRef.following;
    }
  public  void findcampswicth()
    {
        findcamp = !findcamp;
    }
}
