using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class MeeleAIController : MonoBehaviour
{
    public Animator animator;
    public Transform player;
    private GameObject Player;
    private HealthSystem playerhealth;
    private Enemyref enemyref;
    private float PathUpdateDelay;
    private float attackdelay;
    private AudioSource attackSource;


    //attack varibales
    private Vector3 enemydirection; 
    public Vector3 boxSize = new(1, 1, 1);
    public Color boxColor = Color.red;
    public CapsuleCollider eyesight;


    // Patrol variables
    public float patrolRadius = 10f;  // Radius of the patrol area
    public float patrolWaitTime = 3f; // Time to wait at each patrol point
    private Vector3 patrolDestination;
    public bool isPatrolling = true;
    public bool waitingAtPoint = false;
    private float waitTimer;
    public Vector3 Patrolcenter;
    Vector3 lastplayerpos;

    public float stucktimer=10f;

    // Start is called before the first frame update
    public void Awake()
    {
        enemyref = GetComponent<Enemyref>();
        Player = GameObject.FindGameObjectWithTag("Player");
        player = Player.GetComponent<Transform>();  
        playerhealth = player.GetComponent<HealthSystem>();
        attackSource = enemyref.GetComponent<AudioSource>();
        animator = enemyref.GetComponent<Animator>(); 
        Debug.Log("AWAKED");
       
        // Initialize patrol destination
      
        Patrolcenter =   transform.position;
        patrolDestination = GetRandomPatrolPoint();
       
    }


    // Update is called once per frame
    void Update()
    {
        isPatrolling = !enemyref.playerspotted;

        bool inrange = Vector3.Distance(transform.position, player.position) <= 3f;

        if (enemyref.playerspotted == true)
        {
            enemyref.playerwasspotted = true;
            enemyref.agent.speed = 5;
          

            if (!inrange)
            {
                UpdatePath();
                Debug.Log("Chasing");
            }
            else
            {

                attack();
                Debug.Log("attacking");
            }
        }
        else { Patrol(); }

        if (enemyref.playerwasspotted)
        {
            if (enemyref.playerspotted == false)
            {
                Debug.Log("playerlost");
                lastplayerpos = player.position;
                Patrolcenter = lastplayerpos;
                enemyref.playerwasspotted = false;
                //Instantiate<GameObject>(square,lastplayerpos,Quaternion.identity);  
            }
        }

        if (enemyref.agent.velocity.magnitude <= 0.5f&& !waitingAtPoint&& !enemyref.playerspotted)
        {
            animator.SetBool("Moving", false);
            Debug.Log("checking if stuck");
            stucktimer -= Time.deltaTime;
            if (stucktimer <= 0.1f)
            {
                if (enemyref.agent.velocity.magnitude <= 0.7f)
                {  patrolDestination = GetRandomPatrolPoint();
                Debug.Log("Stuck, Changing point");}
                stucktimer= 10f;
                   
            }

        }
        else { stucktimer = 10f; }
    }
    // Generates a random patrol point within the patrol radius
    Vector3 GetRandomPatrolPoint()
    {
       
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += Patrolcenter; // Center around the patrol center
        NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, patrolRadius, NavMesh.AllAreas);
        enemyref.playerwasspotted = false;
        return navHit.position;
    }
    void Patrol()
    {
        Vector3 Distance = patrolDestination- transform.position;
      //  Instantiate<GameObject>(square,patrolDestination,Quaternion.identity);  
      //  Debug.Log(Distance.magnitude.ToString()) ;
     //   Debug.Log("PAtrolling" + Vector3.Distance(patrolDestination, transform.position).ToString());

        if (!isPatrolling) return; // Skip if we're not in patrolling mode
        enemyref.agent.speed = 2;
        animator.SetBool("Moving", true);
        if  (Distance.magnitude >=3.0f && !waitingAtPoint) {
           // Debug.Log("patrolling to location "+Distance.magnitude.ToString());
            enemyref.agent.SetDestination(patrolDestination);

            if (Distance.magnitude <=3.1f )
             {
                Debug.Log("Close to point");
            waitingAtPoint = true; // Start waiting
            waitTimer = patrolWaitTime;
            
             }
      
        }

        // While waiting at the patrol point
        if (waitingAtPoint)
        {
            
            Debug.Log("WAITING");
            waitatpoint();
        }
  
    }
    void waitatpoint()
    {
        animator.SetBool("Moving", false);
        waitTimer -= Time.deltaTime;
        if (waitTimer <= 0.1f)
        {
            Debug.Log("Moving to next point");
              patrolDestination = GetRandomPatrolPoint(); waitingAtPoint = false;
            

        }
    
    }

    void lookattarget() {
      
        Vector3 playerdirection = player.transform.position- transform.position;
        playerdirection.y = 0;
        Quaternion rotation = Quaternion.LookRotation(playerdirection);
        transform.rotation = Quaternion.Slerp(transform.rotation,rotation,0.2f);

         playerdirection.Normalize() ;
        
        enemydirection = playerdirection;
    
    }
    void UpdatePath()
    {
        animator.SetBool("Moving", true);
        lookattarget();
        if (Time.time >= PathUpdateDelay)
        {
            Debug.Log("Updating enenmy Path");
            PathUpdateDelay = Time.time + enemyref.updatepathdelay;
            enemyref.agent.SetDestination(player.position);
        }
    }


    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 directionToPlayer = other.transform.position - transform.position;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, Mathf.Infinity))
            {
                // Check if the raycast hit the player
                if (hit.collider.CompareTag("Player"))
                {
                    // If we hit the player directly, the enemy can see the player
                    enemyref.playerspotted = true;
                }
           
            }
        }
    }


    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyref.playerspotted = false;
        }
    }

    void attack (){
        Vector3 attacksize = new (1f, 1f, 1f);
        if (Time.time >= attackdelay)
        {
            Debug.Log("attacking");
            attackdelay = Time.time + enemyref.attackdelay;

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("uppercut")) { animator.SetTrigger("attack"); }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("uppercut") )
            {
                animator.SetTrigger("attack");
            }

                if (Physics.BoxCast(transform.position, attacksize, enemydirection, out RaycastHit hit, Quaternion.identity, 2f))
                {
                    if (hit.collider.gameObject.name == "Player") { Debug.Log("Hit an object with BoxCast: " + hit.collider.name + hit.transform.gameObject.name); }


                    animator.SetTrigger("attack"); playerhealth.ChangeHealth(-15); attackSource.Play();
                }
            
        }


    }
    private void OnDrawGizmos()
    {
        // Start position of the box cast
        Vector3 startPos = transform.position;
        // End position of the box cast
        Vector3 endPos = startPos + enemydirection.normalized * 2f;

        // Draw the starting box
        Gizmos.color = boxColor;
        Gizmos.matrix = Matrix4x4.TRS(startPos, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);

        // Draw the ending box (after the box cast)
        Gizmos.matrix = Matrix4x4.TRS(endPos, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);

        // Draw a line connecting the start and end positions
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.DrawLine(startPos, endPos);
    }
}

