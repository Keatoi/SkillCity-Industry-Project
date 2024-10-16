using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class NpcController : MonoBehaviour
{
    private AiRef AiRef;
    private GameObject Player;
    private Transform player;
    private GameObject campcenter;
    private float PathUpdateDelay;
    private float attackdelay;
    private AudioSource audioSource;
    private Animator animator;
    public GameObject NPCcanvas;
    private bool talkingtoNpc= false;
    public GameObject talkinstruction;
    public PlayerInput playerInput;

    //Bools
    public bool findcamp, patrol, waitingAtPoint,following;
    public float waitTimer; public float patrolWaitTime = 3f; public float stucktimer = 10f;
    private Vector3 patrolDestination;
    public float patrolradius = 300f;

    public void Awake()
    {
        AiRef = GetComponent<AiRef>();
        Player = GameObject.FindGameObjectWithTag("Player");
        player = Player.GetComponent<Transform>();
        audioSource = AiRef.GetComponent<AudioSource>();
        animator = AiRef.GetComponent<Animator>();
        campcenter = GameObject.FindGameObjectWithTag("CampCenter");
        talkinstruction.SetActive(false);
        NPCcanvas.SetActive(false);
        Debug.Log("AWAKED");
    }

    public void Update()
    {

        if (following)
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
            AiRef.agent.stoppingDistance = 1;
            Patrol();
            if (waitingAtPoint)
            {
                Waitatpoint();
            }

        }


        if (AiRef.agent.velocity.magnitude <= 0.7f && !waitingAtPoint && patrol)
        {

            Debug.Log("checking if stuck");
            stucktimer -= Time.deltaTime;
            if (stucktimer <= 0.1f)
            {
                
                    
                    Debug.Log("Stuck, Changing point");
                Patrol();
               
                stucktimer = 10f;

            }

        }
        else { stucktimer = 10f; }

        if (Raycastcheck.hitpos.collider != null && Raycastcheck.hitpos.collider.gameObject.CompareTag("NPC"))
            {
                talkinstruction.SetActive(!talkingtoNpc);

                // Debug.Log("looking at " + Raycastcheck.hitpos.collider.gameObject.name + talkingtoNpc);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    NPCTALK();

                }

            } else {talkinstruction.SetActive(talkingtoNpc); }

        
       

    }
   public void NPCTALK()
    {
        
        Debug.Log("TALK");talkingtoNpc = !talkingtoNpc; 
        if (!talkingtoNpc) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;playerInput.ActivateInput(); }
        else { Cursor.lockState = CursorLockMode.None; Cursor.visible = true;playerInput.DeactivateInput(); }
        
        NPCcanvas.SetActive(talkingtoNpc);
        talkinstruction.SetActive(talkingtoNpc);


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
        NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, patrolradius, NavMesh.AllAreas);
        //Instantiate<GameObject>(campcenter, navHit.position, Quaternion.identity);
        return navHit.position;
        //   Patrol(navHit.position);

    }
    void Patrol( )
    {Vector3 patrolDestination= GetRandomPatrolPoint();
        Vector3 Distance = patrolDestination - transform.position;
        // Instantiate<GameObject>(campcenter,patrolDestination,Quaternion.identity);  
        //  Debug.Log(Distance.magnitude.ToString()) ;
           Debug.Log("PAtrolling" + Vector3.Distance(patrolDestination, transform.position).ToString());

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
        //Debug.Log("waiting");
        waitTimer -= Time.deltaTime;
        if (waitTimer <= 0)
        {
            Debug.Log("Moving to next point");
            GetRandomPatrolPoint(); waitingAtPoint = false;
            waitTimer = 3f;

        }

    }


    //for UI
  public    void followingswicth()
  {
        following = !following;
        findcamp = false;
  }
  public  void findcampswicth()
    {
        findcamp = !findcamp;
        following= false;
    }
}
