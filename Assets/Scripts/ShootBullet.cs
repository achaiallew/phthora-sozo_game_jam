using UnityEngine;

public class ShootBullet : MonoBehaviour
{
    
    public float bulletSpeed;
    private Rigidbody bulletRB;
    private GameObject player;

    private EnemyController enemyController;


       void Start()
    {
        bulletRB = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        Vector3 direction = transform.rotation * Quaternion.Euler(player.transform.position) * Vector3.back;
        bulletRB.AddForce(direction*bulletSpeed, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other) {

        if (other.gameObject.CompareTag("Enemy"))
        {
            enemyController = other.gameObject.GetComponent<EnemyController>();
            float dmg = Random.Range(20f, 40f);
            enemyController.TakeDamage(dmg);
        }
        if (!other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
         
    }
}
