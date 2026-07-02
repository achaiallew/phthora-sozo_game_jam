using Unity.Cinemachine;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Vector3 playerPos;

    private Animator animator;

    public float doorDist = 3.0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("openDoor", false);
        playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    void Update()
    {
        //TODO: Animate Open Door, when player if in Vicinity
        // float xPos = (playerPos - gameObject.transform.position).Abs().x ;
        // float zPos= (playerPos- gameObject.transform.position).Abs().z;
       
        // if ((xPos < doorDist) || (zPos < doorDist))
        // {
        //     Debug.Log("TriggerDoorAnim");
        //     animator.SetBool("openDoor", true);
        // }

        // if (animator.GetBool("openDoor"))
        // {
        //     if ((xPos > doorDist) || (zPos > doorDist))
        //     {
        //         animator.SetBool("openDoor", false);
        //     }
        // }    
    }
}
