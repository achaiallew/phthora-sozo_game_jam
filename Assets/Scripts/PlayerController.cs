using UnityEngine;


public class PlayerController : MonoBehaviour
{
   
    // Speed of Player
    public float playerSpeed;


    private Rigidbody playerRB;
    private InputSystem_Actions controls;
    private Vector2 moveInput;
    private Vector2 turnInput;
    public float mouseSens;

    public float jumpForce;
    public float jumpTiming;
    private bool isGround = true;

    private Animator playerAnim;

    public GameObject bulletSpawn;
    public GameObject bullet;


    void Awake()
    {
        // Allocate Player Rigidbody
        playerRB = GetComponent<Rigidbody>();

        controls = new InputSystem_Actions();

        playerAnim = GetComponent<Animator>();

    }

    void OnEnable()
    {
        controls.Enable();
    }


    void Update()
    {
        moveInput = controls.Player.Move.ReadValue<Vector2>();
        turnInput = controls.Player.Look.ReadValue<Vector2>();

        transform.Rotate(turnInput.x * transform.up*mouseSens);
        
        if (controls.Player.Move.IsInProgress())
        {
            //Debug.Log("Walk");
            playerAnim.SetBool("walkHold", true);
        } else {    playerAnim.SetBool("walkHold", false);  }
  

        if (controls.Player.Jump.triggered && isGround)
        {
            Debug.Log("Jump");
            
            isGround = false;
            playerAnim.SetTrigger("jumpTrig");
            Invoke("Jump", jumpTiming);
        }

        if (controls.Player.Crouch.IsPressed())
        {
            Debug.Log("Crouch");             
        } else if (controls.Player.Crouch.WasReleasedThisFrame()){
            Debug.Log("Uncrouch");         
        }

        if (controls.Player.Sprint.IsPressed())
        {
            //Debug.Log("Sprint");  
            playerAnim.SetBool("sprintHold", true);           
        } else if (controls.Player.Sprint.WasReleasedThisFrame()){ 
            //Debug.Log("Unsprint");      
            playerAnim.SetBool("sprintHold", false);    
        }

        if (controls.Player.Attack.triggered)
        {
            Debug.Log("Shoot!");
            Instantiate(bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        }
    }

    void FixedUpdate()
    {
        Vector3 move = transform.right * moveInput.x +
                        transform.forward * moveInput.y;
        
        //transform.Translate(move * playerSpeed * Time.deltaTime);
        //Debug.Log(moveInput);
        playerRB.linearVelocity = new Vector3(move.x * playerSpeed, playerRB.linearVelocity.y,move.z * playerSpeed);
        
        
    }

    void Jump()
    {
        playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        isGround = true;
    }


    // private CharacterController controller;

    // void Start()
    // {
    //     controller = GetComponent<CharacterController>();
    // }

    // void Update()
    // {
    //     moveInput = controls.Player.Move.ReadValue<Vector2>();
    //     Vector3 move = transform.right * moveInput.x +
    //                     transform.forward * moveInput.y;

    //     controller.Move(move * playerSpeed * Time.deltaTime);
    // } 
}