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

    public enum enemystate { normal, alerted, chasing, searching }
    public enemystate currentState;

    public float waitToChase, waitToSearch;
    private float chaseCounter, searchCounter;
    public float searchLength;
    private Vector3 searchPosition;

    private float lookAroundCounter,lookAroundAngle;
    public float searchTurnSpeed;
    public float chaseSpeed,walkSpeed;
    private float chaseAdjestCounter;
    
    public float timeToShoot, moveAfterShootTime;
    private float shootWaitCounter, shootMoveCounter, waiToShoot;
    public Transform shootPoint;
    public GameObject projectile;
    void Start()
    {

        //FindFirstObjectByType//

        patrolPointsHolder.SetParent(null);

        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[0].position);

            waitCounter = pointWaitTime;

            anim.SetBool("Walking", true);

        }

            player = FindAnyObjectByType<PlayerController>().transform;
    
    }

    // Update is called once per frame
    void Update()
    {
        CheckForPlayer();

        switch (currentState)
        {
            case (enemystate.normal):

                if (patrolPoints.Length > 0)
                {

                    if (Vector3.Distance(transform.position, new Vector3(patrolPoints[currentPatrol].position.x, transform.position.y, patrolPoints[currentPatrol].position.z)) < .1f)
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

                    if (canSeePlayer)
                    {
                        currentState = enemystate.alerted;

                        anim.SetBool("Walking",false);

                        agent.isStopped = true;

                        chaseCounter = waitToChase;
                        searchCounter = waitToSearch;

                        searchPosition = player.position;

                    }
                }
                break;

            case (enemystate.alerted):

                if (canSeePlayer)
                {
                    searchPosition = player.position;

                    transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

                    chaseCounter -= Time.deltaTime;

                    if (chaseCounter <= 0 || Vector3.Distance(transform.position,player.position) < playerViewRange * .5f)
                    {

                        StartChasing();
                    }

                }
                else
                {
                    searchCounter -= Time.deltaTime;

                    if(searchCounter <= 0)
                    {
                        StartSearching();
                    }

                }
                break;

            case enemystate.chasing:

                if (shootWaitCounter > 0)
                {
                    // Keep updating the target while preparing to shoot
                    if (canSeePlayer)
                    {
                        chaseAdjestCounter -= Time.deltaTime;

                        if (chaseAdjestCounter <= 0)
                        {
                            agent.SetDestination(player.position);
                            chaseAdjestCounter = 0.2f;
                        }
                    }

                    shootWaitCounter -= Time.deltaTime;

                    if (shootWaitCounter <= 0)
                    {
                        shootMoveCounter = moveAfterShootTime;

                        agent.isStopped = true;

                        anim.SetBool("Chasing", false);
                        anim.SetTrigger("Aiming");

                        waiToShoot = moveAfterShootTime * .25f;
                    }

                }
                else
                {
                    shootMoveCounter -= Time.deltaTime;

                    if (shootMoveCounter <= 0)
                    {
                        shootWaitCounter = timeToShoot;

                        agent.isStopped = false;
                        agent.speed = chaseSpeed;
                        agent.SetDestination(player.position);

                        anim.SetBool("Chasing", true);

                        chaseAdjestCounter = 0.2f;
                    }

                    if (waiToShoot > 0)
                    {
                        waiToShoot -= Time.deltaTime;
                        if (waiToShoot <= 0)
                        {
                           
                            anim.SetTrigger("Shoot");

                            Instantiate(projectile, shootPoint.position, shootPoint.rotation);
                        }
                    }

                    transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                }

                if (!canSeePlayer)
                {
                    searchPosition = player.position;

                    anim.SetBool("Chasing",true);

                    StartSearching();
                }
                    break;
                
            case (enemystate.searching):

                if(Vector3.Distance(transform.position, searchPosition) < .5f)
                {
                    agent.isStopped = true;
                    
                    anim.SetBool("Walking", false);

                    anim.SetBool("Chasing", false);


                    searchCounter -= Time.deltaTime;
                    if (searchCounter <= 0)
                    {
                        currentState = enemystate.normal;

                        agent.isStopped = false;

                        agent.SetDestination(patrolPoints[currentPatrol].position);
                        
                        anim.SetBool("Walking", true);

                        agent.speed = walkSpeed;
                    }

                    lookAroundCounter -= Time.deltaTime;

                    if (lookAroundCounter <= 0)
                    {
                        lookAroundCounter = Random.Range(searchLength * .25f, searchLength * .5f);

                        lookAroundAngle = Random.Range(-180f, 180f);
                    }

                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0f, lookAroundAngle, 0f), searchTurnSpeed * Time.deltaTime);

                }

                if (canSeePlayer)
                {

                    StartChasing();
                }
                    break;

        }

    }

    void StartChasing()
    {
        currentState = enemystate.chasing;

        anim.SetBool("Chasing", true);
        anim.SetBool("Walking", false);

        agent.SetDestination(player.position);
        agent.isStopped = false;
        agent.speed = chaseSpeed;

        chaseAdjestCounter = .2f;

        shootWaitCounter = timeToShoot;
    }
    void StartSearching()
    {
        currentState = enemystate.searching;

        searchCounter = searchLength;

        agent.SetDestination(searchPosition);
        agent.isStopped = false;

        anim.SetBool("Walking", true);

        lookAroundCounter = 0f;
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

        if (Vector3.Distance(transform.position, player.position) < playerViewRange)
        {
            //Debug.Log("Player in range");

            Vector3 dirToPlayer = (player.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToPlayer) < playerViewAngle * .5f)
            {

                RaycastHit hit;

                if (Physics.Raycast(transform.position + new Vector3(0f, 1.5f, 0f), dirToPlayer, out hit, playerViewRange))
                {
                    
                    if(hit.collider.tag == "Player")
                    {
             
                        canSeePlayer = true;
                    
                    }
                }

                Debug.DrawRay(transform.position + new Vector3(0f, 1.5f, 0f), dirToPlayer * playerViewRange, Color.purple);
            }

        }
    }
    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.red;

        Vector3 endPosistion = Vector3.zero;

        endPosistion.y = transform.position.y;
        endPosistion.x = transform.position.x + Mathf.Sin(Mathf.Deg2Rad * ((playerViewAngle * .5f) + transform.rotation.eulerAngles.y)) * playerViewRange;
        endPosistion.z = transform.position.z + Mathf.Cos(Mathf.Deg2Rad * ((playerViewAngle * .5f) + transform.rotation.eulerAngles.y)) * playerViewRange;

        Gizmos.DrawLine(transform.position, endPosistion);

        endPosistion.x = transform.position.x + Mathf.Sin(Mathf.Deg2Rad * ((-playerViewAngle * .5f) + transform.rotation.eulerAngles.y)) * playerViewRange;
        endPosistion.z = transform.position.z + Mathf.Cos(Mathf.Deg2Rad * ((-playerViewAngle * .5f) + transform.rotation.eulerAngles.y)) * playerViewRange;

        Gizmos.DrawLine(transform.position, endPosistion);

    }
}

