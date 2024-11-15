using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class NpcController : MonoBehaviour
{
    public AiRef AiRef;
    public GameObject Player;
    private Transform player;
    private GameObject campcenter;
    private float PathUpdateDelay;
    private float attackdelay;
    private AudioSource audioSource;
    private Animator animator;
    private GameObject Enemy = null;
    private AiRef EnemyRef = null;
    public bool enemmyspotted = false;
    public NpcUicontroller npcUicontroller;

    //Bools
    public bool findcamp, patrol, waitingAtPoint, following, rescued;
    public float waitTimer; public float patrolWaitTime = 3f; public float stucktimer = 10f;
    private Vector3 patrolDestination;
    public float patrolradius = 300f;
    private bool inrange;
    Vector3 Enemydirection;
    public Quaternion orientation = Quaternion.identity;

    public void Awake()
    {
        AiRef = GetComponent<AiRef>();if(AiRef == null) { AiRef.GetComponent<AiRef>(); }
        Player = GameObject.FindGameObjectWithTag("Player");
        player = Player.GetComponent<Transform>();
        audioSource = AiRef.GetComponent<AudioSource>();
        animator = AiRef.GetComponent<Animator>();
        campcenter = GameObject.FindGameObjectWithTag("CampCenter");
        Debug.Log("AWAKED");
    }

    public void Update()
    {
        AiRef.Rescued = rescued;
        if (!AiRef.Rescued) { return; }
        bool moving = AiRef.agent.velocity.magnitude >= 1f;
        Debug.Log(" velo" + AiRef.agent.velocity.magnitude + moving);
        if (following)
        {
            Following();
            AiRef.agent.stoppingDistance = 3f;

        }
        else if (findcamp)
        {
            Findcamp();
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
        else if (enemmyspotted == true)
        {
            AiRef.agent.speed = 5;

            if (Enemy != null)
            {
                inrange = Vector3.Distance(transform.position, Enemy.transform.position) <= 2f;
            }

            if (!inrange)
            {
                UpdatePath();
                //  Debug.Log("Chasing");
            }
            else
            {
                Debug.Log("inrage");
                lookattarget();
                attack();
                // Debug.Log("attacking");
            }
        }

        if (AiRef.agent.velocity.magnitude <= 0.7f && !waitingAtPoint && patrol)
        {

            //Debug.Log("checking if stuck");
            stucktimer -= Time.deltaTime;
            if (stucktimer <= 0.1f)
            {


                //  Debug.Log("Stuck, Changing point");
                Patrol();

                stucktimer = 10f;

            }

        }
        else { stucktimer = 10f; }

        if (moving)
        {
            animator.SetBool("moving",true);
        }else animator.SetBool("moving", false);
    }



    //////////////Following state////////////////////////////////////////////////////////////////////////////////////////////////////
    void Following()
    {
        if (Time.time >= PathUpdateDelay)
        {
            // Debug.Log("Updating AI Path");
            PathUpdateDelay = Time.time + AiRef.updatepathdelay;
            AiRef.agent.SetDestination(player.position);
        }
    }

    //////////////Go to camp state////////////////////////////////////////////////////////////////////////////////////////////////////

    public void Findcamp()
    {
        if (campcenter == null)
        {
            campcenter = GameObject.FindGameObjectWithTag("CampCenter");

            if (campcenter == null)
            {
                npcUicontroller.NocampText();
                findcamp = false;
                return;
            }
        }
        else { GoToCamp(); }
    }

    void GoToCamp()
    {
        if (campcenter != null)
        {
            if (Time.time >= PathUpdateDelay)
            {
                // Debug.Log("Path Going To Camp Center");
                PathUpdateDelay = Time.time + AiRef.updatepathdelay;
                AiRef.agent.SetDestination(campcenter.transform.position);
            }
            if (Vector3.Distance(transform.position, campcenter.transform.position) <= 3f)
            {
                findcamp = false; patrol = true;
            }
        }
    }

    //////////////Patrol states////////////////////////////////////////////////////////////////////////////////////////////////////
    Vector3 GetRandomPatrolPoint()
    {

        Vector3 randomDirection = Random.insideUnitSphere * 10;
        randomDirection += campcenter.transform.position; // Center around the patrol center
        NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, patrolradius, NavMesh.AllAreas);
        //Instantiate<GameObject>(campcenter, navHit.position, Quaternion.identity);
        return navHit.position;
        //   Patrol(navHit.position);

    }
    void Patrol()
    {
        Vector3 patrolDestination = GetRandomPatrolPoint();
        Vector3 Distance = patrolDestination - transform.position;
        // Instantiate<GameObject>(campcenter,patrolDestination,Quaternion.identity);  
        //  Debug.Log(Distance.magnitude.ToString()) ;
        //    Debug.Log("PAtrolling" + Vector3.Distance(patrolDestination, transform.position).ToString());

        if (!patrol) return; // Skip if we're not in patrolling mode


        if (Distance.magnitude <= 2f && !waitingAtPoint)
        {
            // Debug.Log("patrolling to location "+Distance.magnitude.ToString());
            AiRef.agent.SetDestination(patrolDestination);

            if (Distance.magnitude <= 1f)
            {
                //  Debug.Log("Close to point");
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

    //////////////Chasing enemy state////////////////////////////////////////////////////////////////////////////////////////////////////
    void lookattarget()//lets Ai face the enemy whilst its moving
    {
        if (Enemy != null)
        {
            Enemydirection = Enemy.transform.position - transform.position;
            Enemydirection.y = 2;
            Quaternion rotation = Quaternion.LookRotation(Enemydirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.2f);
            Enemydirection.Normalize();
        }
        else enemmyspotted = false;
        
    }
    void UpdatePath()
    {
        //animator.SetBool("Moving", true);
        lookattarget();
        if (Time.time >= PathUpdateDelay)
        {
            Debug.Log("Updating NPc Path");
            PathUpdateDelay = Time.time + AiRef.updatepathdelay;
            AiRef.agent.SetDestination(Enemy.transform.position);
        }
    }

    void attack()
    {
        Vector3 rotatedCenter = transform.position + transform.rotation * Vector3.zero;
        Quaternion rotatedOrientation = transform.rotation * orientation;
        if (Time.time >= attackdelay)
        {
          

            attackdelay = Time.time + AiRef.attackdelay;
             Vector3 boxHalfExtents = Vector3.one;
             animator.SetTrigger("attacking");

            if (Physics.BoxCast(rotatedCenter, boxHalfExtents, Enemydirection.normalized, out RaycastHit hit, rotatedOrientation, 2f))
            {
                Debug.Log("ATT box cast");
                if (hit.collider.gameObject.name == "Enemy") { Debug.Log("Hit an object with BoxCast: " + hit.collider.name + hit.transform.gameObject.name); }

                Debug.Log("attacking");

                if (hit.collider.GetComponent<AiRef>()&& hit.collider.gameObject.activeSelf)
                {
                    EnemyRef.changehealthNPC(10, this); if (EnemyRef.health <= 0)
                    {
                        enemmyspotted = false;
                    }
                }
            }

        }


    }
    //////////////NPC Sightline///////////////////////////////////////////////////////////////////////////////////////////

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy")&& other.gameObject.activeSelf)
        {

            Enemy = other.gameObject;
            enemmyspotted = true;
            findcamp = false; patrol = false; waitingAtPoint = false; following = false;
            EnemyRef = Enemy.GetComponent<AiRef>();
            //Debug.Log("enemy in sight" + Enemy.name + other.name);

        }
    }


    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemmyspotted = false;
            Enemy = null;
        }
    }


}
