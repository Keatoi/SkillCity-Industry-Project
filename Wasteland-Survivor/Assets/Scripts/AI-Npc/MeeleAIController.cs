using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class MeeleAIController : MonoBehaviour
{
    private Animator animator;
    private Transform victimtrans;
    private GameObject Victim;
    private HealthSystem playerhealth;
    private AiRef enemyref;
    private AiRef aiRef;
    private float PathUpdateDelay;
    private float attackdelay;
    private AudioSource attackSource;

    //attack varibales
    private Vector3 enemydirection;
    private Vector3 boxSize = new(1, 1, 1);
    private Color boxColor = Color.red;


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

    // Start is called before the first frame update
    public void Awake()
    {
        enemyref = GetComponent<AiRef>();
        Victim = GameObject.FindGameObjectWithTag("Player");
        victimtrans = Victim.GetComponent<Transform>();
        playerhealth = Victim.GetComponent<HealthSystem>();
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
        isPatrolling = !enemyref.playerspotted;

        if (victimtrans == null) { Victim = GameObject.FindGameObjectWithTag("Player"); victimtrans = Victim.transform; }
        bool inrange = Vector3.Distance(transform.position, victimtrans.position) <= 3f;
        Debug.Log("INRAGE" + inrange + Vector3.Distance(transform.position, victimtrans.position).ToString());
        if (enemyref.playerspotted == true)
        {
            enemyref.playerwasspotted = true;
            enemyref.agent.speed = 5;


            if (!inrange)
            {
                UpdatePath();
              //  Debug.Log("Chasing");
            }
            else
            {
                lookattarget();
                attack();
             //   Debug.Log("attacking");
            }
        }
        else { Patrol(); }

        if (enemyref.playerwasspotted)
        {
            if (enemyref.playerspotted == false)
            {
                Debug.Log("playerlost");
                lastplayerpos = victimtrans.position;
                Patrolcenter = lastplayerpos;
                enemyref.playerwasspotted = false;
                //Instantiate<GameObject>(square,lastplayerpos,Quaternion.identity);  
            }
        }

        if (enemyref.agent.velocity.magnitude <= 0.5f && !waitingAtPoint && !enemyref.playerspotted)
        {
            animator.SetBool("Moving", false);
          //  Debug.Log("checking if stuck");
            stucktimer -= Time.deltaTime;
            if (stucktimer <= 0.1f)
            {
                if (enemyref.agent.velocity.magnitude <= 0.7f)
                { patrolDestination = GetRandomPatrolPoint();
                    Debug.Log("Stuck, Changing point"); }
                stucktimer = 10f;

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
        Vector3 Distance = patrolDestination - transform.position;
        //  Instantiate<GameObject>(square,patrolDestination,Quaternion.identity);  
        //  Debug.Log(Distance.magnitude.ToString()) ;
        //   Debug.Log("PAtrolling" + Vector3.Distance(patrolDestination, transform.position).ToString());

        if (!isPatrolling) return; // Skip if we're not in patrolling mode
        enemyref.agent.speed = 2;
        animator.SetBool("Moving", true);
        if (Distance.magnitude >= 3.0f && !waitingAtPoint) {
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

        //    Debug.Log("WAITING");
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
        lookattarget();
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
            Vector3 directionToPlayer = other.transform.position - transform.position;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, Mathf.Infinity))
            {
                // Check if the raycast hit the player
                if (hit.collider.CompareTag("Player"))
                {
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

    void attack() {

        Vector3 halfExtents = new(5f / 2, 5f / 2, 5f / 2);
        Vector3 startpos = transform.position;startpos.y += 1;
  
        if (Time.time >= attackdelay)
        {
            Debug.Log("attacking");
            attackdelay = Time.time + enemyref.attackdelay;
  


            if (Physics.BoxCast(startpos, halfExtents, enemydirection.normalized, out RaycastHit hit, transform.rotation, 1f))
            {
                if (hit.collider.gameObject.name == "Player")
                {
                    Debug.Log("Hit an object with BoxCast: " + hit.collider.name + hit.transform.gameObject.name);

                    animator.SetTrigger("attack"); attackSource.Play();

                    playerhealth.ChangeHealth(-15);
                }

                if (hit.collider.gameObject.CompareTag("NPC"))
                {
                    Debug.Log("Hit an object with BoxCast: " + hit.collider.name + hit.transform.gameObject.name);
                    aiRef.changehealthAi(10); if (aiRef.health <= 0) { enemyref.playerspotted = false; enemyref.playerwasspotted = false; }
                    animator.SetTrigger("attack"); attackSource.Play();
                    
                    
                    


                }
                Rigidbody rb = GetComponent<Rigidbody>(); 
                Vector3 knockbackDirection = (transform.position - victimtrans.position).normalized;
                rb.AddForce(knockbackDirection * 10, ForceMode.Impulse);
            }



        }
      
    }  private void OnDrawGizmos()
        {
            // Start position of the box cast
            Vector3 startPos = transform.position;
        startPos += enemydirection;
        startPos.y += 1;
            // End position of the box cast
            Vector3 endPos = startPos + enemydirection.normalized * 2f;

            // Draw the starting box
            Gizmos.color = boxColor;
            Gizmos.matrix = Matrix4x4.TRS(startPos, transform.rotation,  new(5f / 2, 5f / 2, 5f / 2));
            Gizmos.DrawWireCube(Vector3.zero, boxSize);

            // Draw the ending box (after the box cast)
          //  Gizmos.matrix = Matrix4x4.TRS(endPos, transform.rotation, Vector3.one);
        //    Gizmos.DrawWireCube(Vector3.zero, boxSize);

            // Draw a line connecting the start and end positions
         //   Gizmos.matrix = Matrix4x4.identity;
          //  Gizmos.DrawLine(startPos, endPos);
        }
}