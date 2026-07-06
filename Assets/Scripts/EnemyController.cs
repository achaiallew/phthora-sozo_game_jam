using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent nav;
    private Animator enemyAnim;
    private List<Transform> patrolRoute;

    private int currentIndex = 0;

    private GameObject player;

    [SerializeField] private float enemySpeed = 2.0f;
    [SerializeField] private float attackRange = 4.0f;

    public bool detectPlayer = false;

    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject bulletSpawn;

    private bool shootPlayer = false;

    [SerializeField] private float enemyHealth = 100;


    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        enemyAnim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void SetPatrolRoute(List<Transform> route)
    {
        patrolRoute = route;
    }

    void Update()
    {
        if (patrolRoute == null || patrolRoute.Count == 0) return;

        if (detectPlayer)
        {
            nav.ResetPath();
            float playerDist = Vector3.Distance(transform.position, player.transform.position);

            if (playerDist > attackRange)
            {
                nav.isStopped = false;
                nav.destination = player.transform.position;
                enemyAnim.SetBool("patrolWalk", true);
            }
            else
            {
                nav.isStopped = true;
                enemyAnim.SetBool("patrolWalk", false);

                // Rotate to continuously face the player
                Vector3 direction = player.transform.position - transform.position;

                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetRotation,
                        enemySpeed * Time.deltaTime
                    );
                }

                if (!shootPlayer)
                {
                    // Shoot at the player here
                    InvokeRepeating("ShootPlayer", 0.1f, 2f);
                    shootPlayer = true;
                }               
                
            }
             
        } 
        else
        {
            if (!nav.pathPending && nav.remainingDistance < 0.5f)
            {
                currentIndex = (currentIndex + 1) % patrolRoute.Count;
                nav.speed = enemySpeed;
                nav.destination = patrolRoute[currentIndex].position;
                enemyAnim.SetBool("patrolWalk", true);
            }
        }

        if (enemyHealth < 0)
        {
            Debug.Log("Die");
            Destroy(gameObject);
        }

    }

    void ShootPlayer()
    {
        Instantiate(bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            detectPlayer = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            detectPlayer = false;
        }
    }

    public void TakeDamage(float dmg)
    {
        enemyHealth -= dmg;
    }
}