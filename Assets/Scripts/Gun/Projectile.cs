using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float coolDownFactor = 0.2f;

    Rigidbody rb;
    int playerMask;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerMask = ~(1 << LayerMask.NameToLayer("Player"));
    }

    public void Fire(Vector3 fireDir, float fireSpeed = 2000) => rb.AddForce(fireDir * fireSpeed);


    private IEnumerator OnCollisionEnter(Collision collision)
    {
        // stop physics
        //play animation
        GameObject other = collision.collider.gameObject;
        if (other.CompareTag("Core"))
        {
            other.GetComponent<Core>().Cooldown(coolDownFactor);
        }

        yield return new WaitForEndOfFrame();
        Kill();
    }

    public void Kill()
    {
        // recycle projectile back into pool
        Destroy(gameObject);
    }
}
