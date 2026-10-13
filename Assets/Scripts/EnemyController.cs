using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    public NavMeshAgent agent;

    public Animator anim;
    public Transform patrolPointsHolder;
    public Transform[] patrolPoints;
    private int currentPatrol;
    public float pointWaitTime;
    private float waitCounter;
     
    public int health = 3;
    public Transform player;

    private bool canSeePlayer;
    public float playerViewAngle,playerViewRange;   
    void Start()
    {

        //FindFirstObjectByType//
        player =FindAnyObjectByType<PlayerController>().transform;

        patrolPointsHolder.SetParent(null);

        agent.SetDestination(patrolPoints[0].position);

        waitCounter = pointWaitTime;

        anim.SetBool("Walking", true);

    }

    // Update is called once per frame
    void Update()
    {
        CheckForPlayer();

        if (Vector3.Distance(transform.position,new Vector3 (patrolPoints[currentPatrol].position.x,transform.position.y,patrolPoints[currentPatrol].position.z)) < .1f)
        {
            anim.SetBool("Walking", false);

            waitCounter -= Time.deltaTime;
            
            if (waitCounter <= 0)
            {
                waitCounter = pointWaitTime;

                currentPatrol++;

                if (currentPatrol >= patrolPoints.Length)
                {
                    currentPatrol = 0;
                }

                anim.SetBool("Walking", true);

                agent.SetDestination(patrolPoints[currentPatrol].position);

            }
        }
    }

    public void TakeDamage()
    {

        health--;
        if(health <= 0)
        {
            anim.SetTrigger("Death");

            agent.enabled = false;

            enabled = false;

            GetComponent<Collider>().enabled = false;
        }

        //Destroy(gameObject);
    }

    void CheckForPlayer()
    {
        canSeePlayer = false;

        if (Vector3.Distance(transform.position,player.position) < playerViewRange)
        {
            //Debug.Log("Player in range");

            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            
            if (Vector3.Angle(transform.forward,dirToPlayer) < playerViewAngle * .5f)
            {
                canSeePlayer = true;
            }

        }
    }
}
