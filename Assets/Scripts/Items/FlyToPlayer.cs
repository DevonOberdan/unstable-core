using UnityEngine;

public class FlyToPlayer : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float minRandomFactor, maxRandomFactor;

    [SerializeField] bool useGravity;
    [SerializeField] float delayTime = 1f;
    [SerializeField] float speed;

    Rigidbody rb;
    GravityObject gObj;
    float time;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        time = 0;
    }
    private void Awake()
    {
        gObj = GetComponent<GravityObject>();
        gObj.enabled = false;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (useGravity)
        {
            print(rb.velocity.magnitude);
            time += Time.deltaTime;
            if (time > delayTime && gObj && !gObj.enabled)
            {
                gObj.enabled = true;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
    }
}
