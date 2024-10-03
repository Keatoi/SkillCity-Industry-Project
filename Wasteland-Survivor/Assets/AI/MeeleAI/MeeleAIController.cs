using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class MeeleAIController : MonoBehaviour
{
    public Transform player;
    private GameObject Player;
    private HealthSystem playerhealth;
    private Enemyref enemyref;
    private float PathUpdateDelay;
    private float attackdelay;
    private AudioSource attackSource;

    //attack varibales
    private Vector3 enemydirection; 
    public Vector3 boxSize = new Vector3(1, 1, 1);
    public Color boxColor = Color.red;
    public CapsuleCollider eyesight;


    // Patrol variables
    public float patrolRadius = 10f;  // Radius of the patrol area
    public float patrolWaitTime = 3f; // Time to wait at each patrol point
    private Vector3 patrolDestination;
    private bool isPatrolling = true;
    private bool waitingAtPoint = false;
    private float waitTimer;
    public Vector3 Patrolcenter;


    // Start is called before the first frame update
    public void Awake()
    {
        enemyref = GetComponent<Enemyref>();
        Player = GameObject.FindGameObjectWithTag("Player");
        player = Player.GetComponent<Transform>();  
        playerhealth = player.GetComponent<HealthSystem>();
        attackSource = enemyref.GetComponent<AudioSource>();
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
            enemyref.agent.speed = 4;
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
        else { Patrol();}
    }
    // Generates a random patrol point within the patrol radius
    Vector3 GetRandomPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += Patrolcenter; // Center around the patrol center
        NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, patrolRadius, NavMesh.AllAreas);
        return navHit.position;
    }
    void Patrol()
    { Debug.Log("PAtrolling" + Vector3.Distance(patrolDestination, transform.position).ToString());

        if (!isPatrolling) return; // Skip if we're not in patrolling mode
        enemyref.agent.speed = 2;

        if  (Vector3.Distance(patrolDestination, transform.position) >= 2.5f&& !waitingAtPoint) { 
        
            enemyref.agent.SetDestination(patrolDestination);

            if (Vector3.Distance(patrolDestination, transform.position) <=3f )
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
      /*  if (!waitingAtPoint&& enemyref.agent.velocity.magnitude<=0.5f) {
            Debug.Log("checking if stuck");
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0.1f)
            { if (enemyref.agent.velocity.magnitude <= 0.5f){ } 
                patrolDestination = GetRandomPatrolPoint();
                Debug.Log("Stuck, Changing point");
            }
                
        
        }*/
    }
    void waitatpoint()
    {
        waitTimer -= Time.deltaTime;
        if (waitTimer <= 0.1f)
        {
            Debug.Log("Moving to next point");
            waitingAtPoint = false;
            patrolDestination = GetRandomPatrolPoint(); // Get a new patrol point

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
    {   lookattarget();
        if(Time.time >= PathUpdateDelay) {
            Debug.Log("Updating enenmy Path");
            PathUpdateDelay = Time.time+ enemyref.updatepathdelay;
            enemyref.agent.SetDestination(player.position);
        }
        
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) { 
            enemyref.playerspotted = true;
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
            if (Physics.BoxCast(transform.position, attacksize, enemydirection,out RaycastHit hit,Quaternion.identity,2f))
            {   
                if (hit.collider != null) {Debug.Log("Hit an object with BoxCast: " + hit.collider.name + hit.transform.gameObject.name); }
                attackSource.Play();
                playerhealth.ChangeHealth(-15);
            }
        }


    }
   /* private void OnDrawGizmos()
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
    }*/
}

