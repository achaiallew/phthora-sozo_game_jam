using UnityEngine;

public class ShootBullet : MonoBehaviour
{
    
    public float bulletSpeed;
    public Vector3 modelForwardOffset = new Vector3(90f, 0f, 0f); // adjust based on your mesh
    private Rigidbody bulletRB;

    void Start()
    {
        bulletRB = GetComponent<Rigidbody>();
        Quaternion correction = Quaternion.Euler(modelForwardOffset);
        Vector3 fireDirection = transform.rotation * correction * Vector3.back;
        bulletRB.linearVelocity = fireDirection * bulletSpeed;
    }
}
