using Unity.Cinemachine;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private GameObject player;

    private Animator animator;

    public float doorDist = 3.0f;

    private bool playerInRange = false;
    private bool doorOpen = false;



    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("openDoor", false);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
       
        if (playerInRange && !doorOpen)
        {
            animator.SetBool("openDoor", true);
            doorOpen = true;
        }

        if (!playerInRange && doorOpen)
        {
            StartCoroutine(nameof(CloseDoor), 3f);
        }    
    }

    void CloseDoor()
    {
        animator.SetBool("openDoor", false);
        doorOpen = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInRange = false;
        }
    }
}
