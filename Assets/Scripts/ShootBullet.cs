using UnityEngine;

public class ShootBullet : MonoBehaviour
{
    
    public float bulletSpeed;
    public Vector3 forwardCorrection = new Vector3(-90f, 0f, 0f); // adjust based on your mesh
    private Rigidbody bulletRB;

    void Start()
    {
        bulletRB = GetComponent<Rigidbody>();
        Vector3 direction = transform.rotation * Quaternion.Euler(forwardCorrection) * Vector3.forward;
        bulletRB.linearVelocity = direction * bulletSpeed;
    }
}
