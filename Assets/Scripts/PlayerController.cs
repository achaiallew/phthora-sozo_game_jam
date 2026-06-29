using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
   
    // Speed of Player
    public float playerSpeed=10;


    private Rigidbody playerRB;
    private InputSystem_Actions controls;
    private Vector2 moveInput;

    public float jumpForce = 500;
    private bool isGround = true;


    void Awake()
    {
        // Allocate Player Rigidbody
        playerRB = GetComponent<Rigidbody>();

        controls = new InputSystem_Actions();

    }

    void OnEnable()
    {
        controls.Enable();
    }


    void Update()
    {
        moveInput = controls.Player.Move.ReadValue<Vector2>();


        Vector3 move = transform.right * moveInput.y +
                        transform.forward * -moveInput.x;
        
        transform.Translate(move * playerSpeed * Time.deltaTime);
        //Debug.Log(moveInput);
  

        if (controls.Player.Jump.triggered && isGround)
        {
            Debug.Log("Jump");
            playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGround = false;
        }

        if (controls.Player.Crouch.IsPressed())
        {
            Debug.Log("Crouch");             
        } else if (controls.Player.Crouch.WasReleasedThisFrame()){
            Debug.Log("Uncrouch");         
        }

        if (controls.Player.Sprint.IsPressed())
        {
            Debug.Log("Sprint");             
        } else if (controls.Player.Sprint.WasReleasedThisFrame()){
            Debug.Log("Unsprint");         
        }
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

    //     controller.Move(move * 5f * Time.deltaTime);
    // }
}