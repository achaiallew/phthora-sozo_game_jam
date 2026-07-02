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


    private Rigidbody playerRB;
    private Vector2 moveInput;

    public float mouseSens;

    public float jumpForce;
    public float jumpTiming;
    


    private Animator playerAnim;

    public GameObject bulletSpawn;
    public GameObject bullet;


    void Awake()
    {
        // Allocate Player Rigidbody
        playerRB = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

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
        if (inputDir.magnitude > 0.1f)
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
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);   
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
            playerSpeed *= 2f;          
        } else if (sprintAction.WasReleasedThisFrame()){ 
            //Debug.Log("Unsprint");      
            playerAnim.SetBool("sprintHold", false); 
            playerSpeed /= 2f;    
        }
    }
    
    void Kneel()
    {
        if (kneelAction.WasPerformedThisFrame())
        {
            Debug.Log("Crouch"); 
            playerAnim.SetBool("kneelHold", true);            
        } else if (kneelAction.WasReleasedThisFrame()){
            Debug.Log("Uncrouch");      
            playerAnim.SetBool("kneelHold", false);    
        }
    }

    void Shoot()
    {
        if (shootAction.triggered)
        {
            Debug.Log("Shoot!");
            Instantiate(bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        }
    }

}