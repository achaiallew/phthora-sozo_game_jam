using UnityEngine;

public class ShootBullet : MonoBehaviour
{
    // Declare Bullet Variables
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float maxDMG = 40f;
    [SerializeField] private float minDMG = 20f;

    // Declare Bullet Rigidbody
    private Rigidbody bulletRB;
    
    // Direction Correction Variables
    public Vector3 forwardCorrection = new Vector3(-90f, 0f, 0f);
    private Vector3 overideDirection;
    private bool hasOD = false;

    // Reference Player Variable
    private GameObject player;

    // Access Enemy Controller Variable
    private EnemyController enemyController;

    void Awake()
    {
        // Assign Rigidbody
        bulletRB = GetComponent<Rigidbody>();
        // Locate Player Game Object
        player = GameObject.FindGameObjectWithTag("Player");
    }


    void Start()
    {
        // Check for No Override Direction
        if (!hasOD)
        {
            // Determine Bullet Direction 
            Vector3 direction = transform.rotation * Quaternion.Euler(forwardCorrection) * Vector3.forward;
            // Add Bullet Velocity
            SetVelocity(direction.normalized );
        }
    }

    public void Initialize(Vector3 direction)
    {
        // Ensure Valid Direction
        if (direction.sqrMagnitude < 0.001f) return; 

        // Set Override Direction and Add Bullet Velocity
        hasOD = true;
        overideDirection = direction.normalized;
        SetVelocity(overideDirection);
    }

    void SetVelocity(Vector3 direction)
    {
        // Ensure Rigidbody is Assigned
        if (bulletRB == null)
        {
            bulletRB = GetComponent<Rigidbody>();
        }

        // Add Linear Velocity 
        bulletRB.linearVelocity = direction* bulletSpeed;
      }
 

    private void OnCollisionEnter(Collision other) 
    {
        // Check if Bullet Shoots Enemy
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Access Enemy Controller for Collision Enemy
            enemyController = other.gameObject.GetComponent<EnemyController>();
            // Randomise Bullet Damage
            float dmg = Random.Range(minDMG, maxDMG);
            // Damage Enemy
            enemyController.TakeDamage(dmg);

            // Count Player Shots
            player.GetComponent<PlayerController>().shotsOnTarget ++;
        }

        // If Bullet Collides With Anything Except Player
        if (!other.gameObject.CompareTag("Player"))
        {
            // Destroy Bullet
            Destroy(gameObject);
        }
         
    }
}
