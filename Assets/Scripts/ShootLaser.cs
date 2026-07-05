using UnityEngine;

public class ShootLaser : MonoBehaviour
{
    
    public float laserSpeed;
    private Rigidbody laserRB;
    private GameObject player;

    void Start()
    {
        laserRB = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        Vector3 direction = transform.rotation * Quaternion.Euler(player.transform.position) * Vector3.forward;
        laserRB.AddForce(direction*laserSpeed, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other) {

        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Take Damage");// Take Damage (Enemy Health)
        }
        if (!other.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
         
    }
}
