using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float coolDownFactor = 0.2f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private IEnumerator OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.collider.gameObject;
        
        if (other.TryGetComponent(out Core core))
        {
            core.Cooldown(coolDownFactor);
        }

        yield return new WaitForEndOfFrame();
        Kill();
    }

    public void Fire(Vector3 fireDir, float fireSpeed = 2000)
    {
        rb.AddForce(fireDir * fireSpeed);
    }

    public void Kill()
    {
        // recycle projectile back into pool
        Destroy(gameObject);
    }
}
