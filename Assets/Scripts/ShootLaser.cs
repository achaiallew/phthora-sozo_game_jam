using UnityEngine;
using VolumetricLines;

public class ShootLaser : MonoBehaviour
{
    // Declare Laser Variables    
    [SerializeField] private float laserSpeed;
    [SerializeField] private  float laserLength = 100f;
    [SerializeField] private float maxDMG = 20f;
    [SerializeField] private float minDMG = 10f;

    // Declare Laser Rigidbody
    private Rigidbody laserRB;

    // Declare Laser Variable
    private VolumetricLineBehavior laserLine;

    // Declare Laser Direction Variables
    private bool hasDirection = false;
    private Vector3 fireDirection;


    // Reference Player Variable
    private GameObject player;
    // Access Player Controller Variable
    private PlayerController playerController;

    void Awake()
    {
        // Assign Rigidbody
        laserRB = GetComponent<Rigidbody>();
        // Assign Laser Line Behaviour
        laserLine = GetComponentInChildren<VolumetricLineBehavior>();

        // Locate Player Game Object
        player = GameObject.FindGameObjectWithTag("Player");
        // Access Player Controller
        playerController = player.GetComponent<PlayerController>();
    }

    void Start()
    {
        // Check for No Direction
        if (!hasDirection)
        {
            // Assign Forward Z Axis As Direction
            Vector3 direction = transform.forward;
            // Initialise Laser
            Initialize(direction);
        }       
    }

    public void Initialize(Vector3 direction)
    {
        // Reset Vertical Direction
        direction.y = 0f;

        // Ensure Valid Direction
        if (direction.sqrMagnitude < 0.001f) return; 

        // Ensure RigidBody is Setup Correctly
        ConfigureRigidbody();

        // Set Fire Direction 
        hasDirection = true;
        fireDirection = direction.normalized;
        transform.rotation = Quaternion.LookRotation(fireDirection);

        // Apply Visual Direction 
        ApplyVisualDirection(fireDirection);

        // Add Linear Velocity
        laserRB.linearVelocity = fireDirection * laserSpeed;
    }

    void ConfigureRigidbody()
    {
        // Ensure Rigidbody is Assigned
        if (laserRB == null)
        {
            laserRB = GetComponent<Rigidbody>();
        }
        if (laserRB == null) return;

        // Ensure Gravity is Zero
        laserRB.useGravity = false;
        // Ensure Rotation is Freezed
        laserRB.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void ApplyVisualDirection(Vector3 direction)
    {
        // Check Laser Line
        if (laserLine != null)
        {
            // Apply Laser Visual Length
            laserLine.StartPos = Vector3.zero;
            laserLine.EndPos = direction * laserLength;
        }
        else
        {
            // Rotate Laser Line towards Player
            transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
        }
        
    }

    private void OnCollisionEnter(Collision other) 
    {
        // Check if Laser Hits Player
        if (other.gameObject.CompareTag("Player"))
        {
            // Randomise Laser Damage
            float dmg = Random.Range(minDMG, maxDMG);
            // Damage Player
            playerController.TakeDamage(dmg);
            ////Debug.Log("Shoot Player! " + dmg);
        }

        // If Laser Collides With Anything Except Enemy
        if (!other.gameObject.CompareTag("Enemy"))
        {
            // Destroy Laser
            Destroy(gameObject);
        }
         
    }
}
