using Unity.Cinemachine;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private GameObject player;

    private Animator animator;

    public float doorDist = 3.0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("openDoor", false);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        //TODO: Animate Open Door, when player if in Vicinity
        float xPos = (player.transform.position - gameObject.transform.position).Abs().x ;
        float zPos= (player.transform.position- gameObject.transform.position).Abs().z;
       
        if ((xPos < doorDist) || (zPos < doorDist))
        {
            Debug.Log("TriggerDoorAnim");
            animator.SetBool("openDoor", true);
        }

        if (animator.GetBool("openDoor"))
        {
            if ((xPos > doorDist) || (zPos > doorDist))
            {
                animator.SetBool("openDoor", false);
            }
        }    
    }
}
