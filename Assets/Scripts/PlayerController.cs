using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    //TODO: Add Comments to Code
    // Speed of Player
    public InputActionAsset InputActions;
    public CharacterController controller;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction shootAction;
    private InputAction sprintAction;
    private InputAction kneelAction;


    [SerializeField]
    private Transform playerCamera;

    public float playerSpeed = 8.0f;


    private float verticalVelocity = 0f;
    public float jumpHeight = 1.0f;
    private float gravity = -9.8f;


    private Vector2 moveInput;

    public float mouseSens;

    public float jumpForce;
    public float jumpTiming;
    


    private Animator playerAnim;

    public GameObject bulletSpawn;
    public GameObject bullet;

    [SerializeField] private float playerHealth = 100;

    private AudioSource moveSound;
     [SerializeField] private AudioSource runSound;



    void Awake()
    {
        // Allocate Player Rigidbody
        playerAnim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        moveSound = GetComponent<AudioSource>();

        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        shootAction = InputSystem.actions.FindAction("Attack");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        kneelAction = InputSystem.actions.FindAction("Crouch");
    
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
        moveInput = moveAction.ReadValue<Vector2>();

        Move();
        Jump();
        Kneel();
        Shoot();
        AudioController();

        if (playerHealth < 0)
        {
            Debug.Log("Game Over!");
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
            
            playerAnim.SetBool("walkHold", true);
        }
        else
        {
            playerAnim.SetBool("walkHold", false);
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
                //TODO: Fix Jump Anim
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

    void Sprint()
    {
        if (sprintAction.WasPerformedThisFrame())
        {
            //Debug.Log("Sprint");  
            playerAnim.SetBool("sprintHold", true); 
            playerSpeed *= 2.5f;          
        } else if (sprintAction.WasReleasedThisFrame()){ 
            //Debug.Log("Unsprint");      
            playerAnim.SetBool("sprintHold", false); 
            playerSpeed /= 2.5f;    
        }
    }
    
    void Kneel()
    {
        if (kneelAction.WasPerformedThisFrame())
        {
            //Debug.Log("Crouch"); 
            playerAnim.SetBool("kneelHold", true);            
        } else if (kneelAction.WasReleasedThisFrame()){
            //Debug.Log("Uncrouch");      
            playerAnim.SetBool("kneelHold", false);    
        }
    }

    void Shoot()
    {
        if (shootAction.triggered)
        {
            //Debug.Log("Shoot!");
            Instantiate(bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        }
    }

    void JumpVert()
    {
        verticalVelocity = Mathf.Sqrt(jumpHeight * -1f * gravity);
    }

    public void TakeDamage(float dmg)
    {
        playerHealth -= dmg;
        Debug.Log(playerHealth);
    }

    void AudioController()
    {
        if (moveAction.WasPerformedThisFrame() && !sprintAction.WasPerformedThisFrame())
        {
            moveSound.Play();
        } 
        
        if (moveAction.IsInProgress() && sprintAction.WasPerformedThisFrame())
        {
            Debug.Log("Sprint Sound!");
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

}