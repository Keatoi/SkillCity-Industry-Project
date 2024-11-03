using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class MeeleAIController : MonoBehaviour
{
    private Animator animator;
    public Transform victimtrans;
    public GameObject Victim;
    public HealthSystem playerhealth;
    private AiRef enemyref;
    private AiRef aiRef;
    private float PathUpdateDelay;
    private float attackdelay;
    private AudioSource attackSource;

    //attack varibales
    public Vector3 enemydirection;



    // Patrol variables
    private float patrolRadius = 10f;  // Radius of the patrol area
    public float patrolWaitTime = 3f; // Time to wait at each patrol point
    private Vector3 patrolDestination;
    public bool isPatrolling = true;
    public bool waitingAtPoint = false;
    private float waitTimer;
    public Vector3 Patrolcenter;
    Vector3 lastplayerpos;

    public float stucktimer = 10f;

    [Header ("box collider")]
    public Vector3 boxCenter = Vector3.zero;     // Relative to the GameObject's position
    public Vector3 boxHalfExtents = Vector3.one; // Half the size of the box in each dimension
    public Vector3 direction = Vector3.forward;  // Direction of the BoxCast
    public float maxDistance = 5.0f;             // Max distance of the BoxCast
    public Quaternion orientation = Quaternion.identity; // Orientation of the box
    public GameObject Other;


    // Start is called before the first frame update
    public void Awake()
    {
        enemyref = GetComponent<AiRef>();
        Victim = GameObject.FindGameObjectWithTag("Player");
        victimtrans = Victim.GetComponent<Transform>();
      //  playerhealth = Victim.GetComponent<HealthSystem>();
        attackSource = enemyref.GetComponent<AudioSource>();
        animator = enemyref.GetComponent<Animator>();
        Debug.Log("AWAKED");

        // Initialize patrol destination

        Patrolcenter = transform.position;
        patrolDestination = GetRandomPatrolPoint();

    }


    // Update is called once per frame
    void Update()
    {
        isPatrolling = !enemyref.playerspotted&& !enemyref.playerwasspotted;

        if (victimtrans == null) { Victim = GameObject.FindGameObjectWithTag("Player"); victimtrans = Victim.transform; }
        bool inrange = Vector3.Distance(transform.position, victimtrans.position) <= 3f;
        //  Debug.Log("INRAGE" + inrange + Vector3.Distance(transform.position, victimtrans.position).ToString());
        if (enemyref.playerspotted == true|| enemyref.playerwasspotted)
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
                Lookattarget();
                Attack();

            }
        }
        else { Patrol(); }

        if (enemyref.playerwasspotted && ! enemyref.playerspotted)
        {

            stucktimer -= Time.deltaTime;
            if (stucktimer <= 0.1f)
            { 
            
                Debug.Log("playerlost");
                lastplayerpos = victimtrans.position;
                Patrolcenter = lastplayerpos;
                enemyref.playerwasspotted = false;
            }

        }

        if (enemyref.agent.velocity.magnitude <= 0.5f && !waitingAtPoint && !enemyref.playerspotted&& !enemyref.playerwasspotted)
        {
            animator.SetBool("Moving", false);
            Debug.Log("checking if stuck");
            stucktimer -= Time.deltaTime;
            if (stucktimer <= 0.1f)
            {
                
                    patrolDestination = GetRandomPatrolPoint();
                    Debug.Log("Stuck, Changing point");
                
                stucktimer = 10f;

            }

        }
        //else { stucktimer = 10f; }
    }
    // Generates a random patrol point within the patrol radius
    Vector3 GetRandomPatrolPoint()
    {

        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += Patrolcenter; // Center around the patrol center
        NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, patrolRadius, NavMesh.AllAreas);
      //  enemyref.playerwasspotted = false;
        return navHit.position;
    }
    void Patrol()
    {
        Vector3 Distance = patrolDestination - transform.position;
        //  Instantiate<GameObject>(square,patrolDestination,Quaternion.identity);  
        //  Debug.Log(Distance.magnitude.ToString()) ;
        //   Debug.Log("PAtrolling" + Vector3.Distance(patrolDestination, transform.position).ToString());

        if (!isPatrolling) return; // Skip if we're not in patrolling mode
        enemyref.agent.speed = 2;
        animator.SetBool("Moving", true);
        if (Distance.magnitude >= 3.0f && !waitingAtPoint)
        {
            // Debug.Log("patrolling to location "+Distance.magnitude.ToString());
            enemyref.agent.SetDestination(patrolDestination);

            if (Distance.magnitude <= 3.1f)
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
            Waitatpoint();
        }

    }
    void Waitatpoint()
    {
        animator.SetBool("Moving", false);
        waitTimer -= Time.deltaTime;
        if (waitTimer <= 0.1f)
        {
            Debug.Log("Moving to next point");
            patrolDestination = GetRandomPatrolPoint(); waitingAtPoint = false;


        }

    }

    void Lookattarget()
    {

        Vector3 playerdirection = victimtrans.position - transform.position;
        playerdirection.y = 0;
        Quaternion rotation = Quaternion.LookRotation(playerdirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.2f);

        playerdirection.Normalize();

        enemydirection = playerdirection;

    }
    void UpdatePath()
    {
        animator.SetBool("Moving", true);
        Lookattarget();
        if (Time.time >= PathUpdateDelay)
        {
            Debug.Log("Updating enenmy Path");
            PathUpdateDelay = Time.time + enemyref.updatepathdelay;
            enemyref.agent.SetDestination(victimtrans.position);
        }
    }


    public void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("NPC"))
        {
            Victim = other.gameObject;
            victimtrans = other.transform;
       
            if (Victim.TryGetComponent<AiRef>(out AiRef component)) { aiRef = component; };
            if (aiRef.Rescued) { enemyref.playerspotted = true; }

            //  Debug.Log("airef" + Victim.name + aiRef.name);

        }
        else if (other.CompareTag("Player"))
        {
            Vector3 eyespos = new (transform.position.x,transform.position.y+1.5f,transform.position.z);
            Vector3 directionToPlayer = other.transform.position - transform.position;

            Other = other.gameObject;
            if (Physics.Raycast(eyespos, directionToPlayer.normalized, out RaycastHit hit, Mathf.Infinity))
            {
               // Debug.Log(hit.collider.name);
                // Check if the raycast hit the player
                if (hit.collider.CompareTag("Player"))
                {
                    Debug.Log("seeplayer");
                    // If we hit the player directly, the enemy can see the player
                    enemyref.playerspotted = true;
                    Victim = other.gameObject;
                    victimtrans = other.transform;
                }

            }
        }

    }


    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("NPC"))
        {
            enemyref.playerspotted = false;
        }

    }

    void Attack()
    {
      //  Debug.Log("attacking time "+Time.time+" delay "+ attackdelay );
        
         if (Time.time > attackdelay)
        {
            Debug.Log(" meele");
            attackdelay = Time.time + enemyref.attackdelay;
            
            Attackhitbox();

        }


    }

    void Attackhitbox()
    {
        Vector3 rotatedCenter = transform.position + transform.rotation * boxCenter;
        Quaternion rotatedOrientation = transform.rotation * orientation;
       

        // Perform the BoxCast
        if (Physics.BoxCast(rotatedCenter, boxHalfExtents, enemydirection.normalized * maxDistance, out RaycastHit hitInfo, rotatedOrientation, maxDistance))
        {
            if (hitInfo.collider.CompareTag("Player"))
            {
                Debug.Log("hitplayer");
                playerhealth.ChangeHealth(-10);
                animator.SetTrigger("attack");
                attackSource.Play();
            }
            if (hitInfo.collider.CompareTag("NPC"))
            { animator.SetTrigger("attack");
                attackSource.Play();
                aiRef.changehealthAi(10); if (aiRef.health <= 0) { enemyref.playerspotted = false; }
            }
        }
    }
    private void OnDrawGizmos()
    {
        // Rotate boxCenter by the GameObject's rotation to align it correctly
        Vector3 rotatedCenter = transform.position + transform.rotation * boxCenter;
        Quaternion rotatedOrientation = transform.rotation * orientation;
        if (Other != null)
        {
            // Calculate direction from the current object to the player
            Vector3 eyespos = new (transform.position.x,transform.position.y+1.5f,transform.position.z);
            Vector3 directionToPlayer = Other.transform.position - transform.position;

            // Set the gizmo color to distinguish it in the editor
            Gizmos.color = Color.red;

            // Draw a line to represent the ray
            Gizmos.DrawLine(eyespos, transform.position + directionToPlayer.normalized * 100f);
        }
        // Perform the BoxCast
        if (Physics.BoxCast(rotatedCenter, boxHalfExtents, enemydirection.normalized*maxDistance, out RaycastHit hitInfo, rotatedOrientation, maxDistance))
        {
        
          
            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(rotatedCenter + enemydirection , orientation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2);

        }
         else
        {
            // Draw the box at the max distance (indicates no collision)
            Gizmos.color = Color.green;
            Gizmos.matrix = Matrix4x4.TRS(rotatedCenter + enemydirection.normalized * maxDistance, rotatedOrientation, boxHalfExtents);
            Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2);
        }


    }
}
