using UnityEngine;

public class ShootLaser : MonoBehaviour
{
    
    public float laserSpeed;
    private Rigidbody laserRB;
    private GameObject player;
    private PlayerController playerController;

    void Start()
    {
        laserRB = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        Vector3 direction = transform.rotation * Quaternion.Euler(player.transform.position) * Vector3.forward;
        laserRB.AddForce(direction*laserSpeed, ForceMode.Impulse);

        playerController = player.GetComponent<PlayerController>();
    }

    private void OnCollisionEnter(Collision other) {

        if (other.gameObject.CompareTag("Player"))
        {
            float dmg = Random.Range(10f, 20f);
            Debug.Log("Shoot Player! " + dmg);
            playerController.TakeDamage(dmg);
        }
        if (!other.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
         
    }
}
