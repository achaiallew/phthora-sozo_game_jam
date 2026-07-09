using UnityEngine;
using UnityEngine.AI;
using Unity.VisualScripting;
using System.Collections.Generic;

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

    [SerializeField] private GameObject laser;
    [SerializeField] private GameObject laserSpawn;

    private bool shootPlayer = false;
    private bool dead = false;

    [SerializeField] private float enemyHealth = 100;

    private GameManager gameManager;


    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        enemyAnim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        dead = false;
    }

    public void SetPatrolRoute(List<Transform> route)
    {
        patrolRoute = route;
    }

    void Update()
    {
        if (patrolRoute == null || patrolRoute.Count == 0) return;

        if (detectPlayer && !dead)
        {
            nav.ResetPath();
            float playerDist = Vector3.Distance(transform.position, player.transform.position);

            if (playerDist > attackRange)
            {
                StopShooting();
                nav.isStopped = false;
                nav.destination = player.transform.position;
                enemyAnim.SetBool("patrolWalk", true);
            }
            else
            {
                nav.isStopped = true;
                enemyAnim.SetBool("patrolWalk", false);

                AimAtPlayer(false);


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
            StopShooting();
            
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
            dead = true;
            enemyAnim.SetTrigger("isDead");
            gameManager.killCount ++;
        }

        if (!gameManager.gameActive)
        {
            StopShooting();
        }

    }

    void AimAtPlayer(bool snap)
    {
        Vector3 direction = player.transform.position - transform.position;

        // Reset Vertical Direction
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f) return;
        
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = snap
            ? targetRotation
            :Quaternion.Slerp(transform.rotation, targetRotation, enemySpeed * Time.deltaTime);

    }

    void ShootPlayer()
    {
        // Ensure All Variables Are Assigned
        if (player == null || laser == null || laserSpawn == null) return;

        AimAtPlayer(true);

        // Assign Player Poistion as Aim Target
        Vector3 target = player.transform.position;
        // Set Y Value to Match Laser Spawn Point
        target.y = laserSpawn.transform.position.y;
        // Determine Direction (based on difference between player and laser spawn point)
        Vector3 direction = target - laserSpawn.transform.position;

        // Ensure Valid Direction
        if (direction.sqrMagnitude < 0.001f) return;

        // Create Shooting Direction Rotation
        Quaternion laserRotation = Quaternion.LookRotation(direction.normalized);
        // Spawn Laser
        GameObject spawnLaser = Instantiate(laser, laserSpawn.transform.position, laserRotation);
        
        // Access Shoot Laser Script
        ShootLaser shootLaser = spawnLaser.GetComponentInChildren<ShootLaser>();
        // If Script is Located
        if (shootLaser != null)
        {
            //Initalise Laser in Correct Aiming Direction
            shootLaser.Initialize(direction);
        }

        enemyAnim.SetTrigger("shoot");
 
    }

    void StopShooting()
    {
        if (!shootPlayer) return;

        CancelInvoke("ShootPlayer");
        shootPlayer = false;
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
            StopShooting();
        }
    }

    public void TakeDamage(float dmg)
    {
        enemyHealth -= dmg;
    }
}