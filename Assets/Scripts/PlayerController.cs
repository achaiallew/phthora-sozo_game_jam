using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    //TODO: Add Comments to Code
    // Speed of Player
    public InputActionAsset InputActions;
    public CharacterController controller;

    private InputAction moveAction;
    public InputAction jumpAction;
    public InputAction shootAction;
    public InputAction sprintAction;
    private InputAction kneelAction;
    private InputAction interactAction;
    public InputAction reloadAction;


    [SerializeField] private Transform playerCamera;

    public float playerSpeed = 8.0f;


    private float verticalVelocity = 0f;
    public float jumpHeight = 1.0f;
    private float gravity = -9.8f;


    private Vector2 moveInput;

    //public float mouseSens;

    private Animator playerAnim;

    public GameObject bulletSpawn;
    public GameObject bullet;

    public float playerHealth = 100;

    [SerializeField] private AudioSource moveSound;
    [SerializeField] private AudioSource runSound;

    private GameManager gameManager;

    public int shotsTaken;
    public int shotsOnTarget;

    private bool playerInRange = false;


    void Awake()
    {
        playerAnim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        shootAction = InputSystem.actions.FindAction("Attack");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        kneelAction = InputSystem.actions.FindAction("Crouch");
        interactAction = InputSystem.actions.FindAction("Interact");
        reloadAction.Enable();
    
    }

    void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }


    void Update()
    {
        if (gameManager.gameActive)
        {
            moveInput = moveAction.ReadValue<Vector2>();

            Move();
            Jump();
            Kneel();
            Shoot();
            Reload();
            Interact();
            AudioController();
        }   
    }

    
    private void Move()
    {
        float cameraYaw = playerCamera.eulerAngles.y;

        // Body always faces where the camera looks (shooter-style aiming)
        transform.rotation = Quaternion.Euler(0f, cameraYaw, 0f);

        Vector3 verticalMove = new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime;

        // WASD strafes relative to camera facing, not player facing
        Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 moveDirection = Quaternion.Euler(0f, cameraYaw, 0f) * inputDir;

        controller.Move(moveDirection.normalized * playerSpeed * Time.deltaTime + verticalMove);
        
        //TODO: Diff Anim for Diff Directions
        // Animation
        if (inputDir.magnitude > 0.01f)
        {
            
            playerAnim.SetBool("isWalking", true);
        }
        else
        {
            playerAnim.SetBool("isWalking", false);
        }

        // Sprint When Walking Only
        Sprint();
    }
    void Jump()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = -1f;

            if (jumpAction.triggered)
            {   
                playerAnim.SetTrigger("jumpTrig");
                Invoke("JumpVert", 0.2f);   
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime; 
            verticalVelocity = Mathf.Max(verticalVelocity, -20f);
        }
    }

    void JumpVert()
    {
        verticalVelocity = Mathf.Sqrt(jumpHeight * -1f * gravity);
    }

    void Sprint()
    {
        if (sprintAction.WasPerformedThisFrame())
        {
            //Debug.Log("Sprint");  
            playerAnim.SetBool("isSprinting", true); 
            playerSpeed *= 2.5f;          
        } else if (sprintAction.WasReleasedThisFrame()){ 
            //Debug.Log("Unsprint");      
            playerAnim.SetBool("isSprinting", false); 
            playerSpeed /= 2.5f;    
        }
    }
    
    void Kneel()
    {
        if (kneelAction.WasPerformedThisFrame())
        {
            //Debug.Log("Crouch"); 
            playerAnim.SetBool("isKneeling", true);            
        } else if (kneelAction.WasReleasedThisFrame()){
            //Debug.Log("Uncrouch");      
            playerAnim.SetBool("isKneeling", false);    
        }
    }

    void Shoot()
    {
        if (shootAction.triggered)
        {
            // Determine Direction (based on difference between player and laser spawn point)
            Vector3 direction = playerCamera.forward;

            // Ensure Valid Direction
            if (direction.sqrMagnitude < 0.001f)
                return;

            // Create Shooting Direction Rotation
            Quaternion bulletRotation = Quaternion.LookRotation(direction.normalized);

            // Spawn Bullet
            GameObject spawnedBullet = Instantiate(bullet, bulletSpawn.transform.position, bulletRotation);

            // Access Shoot Bullet Script
            ShootBullet shootBullet = spawnedBullet.GetComponentInChildren<ShootBullet>();
            // If Script is Located
            if (shootBullet != null)
            {
                //Initalise Bullet in Correct Aiming Direction
                shootBullet.Initialize(direction);
            }

            // Count Total Shots
            shotsTaken++;
            gameManager.magBullets--;
        }
    }   

    void Reload()
    {
        if (reloadAction.triggered)
        {
            if (gameManager.magBullets != gameManager.magazine)
            {
                if (gameManager.excessBullets > 0)
                {
                    int bulletDiff = gameManager.magazine - gameManager.magBullets;
                    gameManager.magBullets += bulletDiff ;
                    gameManager.excessBullets  -= bulletDiff;
                    //TODO: Reload Sound + Anim
                    playerAnim.SetTrigger("reload");
                }
                
                if (gameManager.excessBullets < 0)
                {
                    gameManager.excessBullets = 0;
                }

                if (shootAction.enabled == false)
                {
                    shootAction.Enable();
                }
            }
            else
            {
                //TODO: Full Mag Sound
            }
        }
    } 

    void Interact()
    {
        if (interactAction.triggered && playerInRange)
        {
            gameManager.PickUpGun();
        }
        
    }

    public void TakeDamage(float dmg)
    {
        playerHealth -= dmg;
       // Debug.Log(playerHealth);
    }

    void AudioController()
    {
        if (moveAction.WasPerformedThisFrame() && !sprintAction.WasPerformedThisFrame())
        {
            moveSound.Play();
        } 
        
        if (moveAction.IsInProgress() && sprintAction.WasPerformedThisFrame())
        {
            //Debug.Log("Sprint Sound!");
            runSound.Play();
            moveSound.Pause();
        }
        
        if (sprintAction.WasReleasedThisFrame())
        {
            runSound.Pause();
            moveSound.Play();
        }

        if (moveAction.WasReleasedThisFrame())
        {
            moveSound.Pause();
        } 
    
    }

    void OnTriggerEnter(Collider other)
    {
        //TODO: PopUp Press E to Interact
        
        if (other.gameObject.CompareTag("Pistol"))
        { 
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Pistol"))
        { 
            playerInRange = false;
        }
    }

}