using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyToPlayer : MonoBehaviour
{
    [SerializeField] GameObject player;
    GravityObject gObj;
    Vector3 vel = Vector3.zero;
    [SerializeField] float minRandomFactor, maxRandomFactor;

    [SerializeField] bool useGravity;

    Rigidbody rb;
    //[SerializeField] float maxVel = 12f;

    float time;
    [SerializeField] float delayTime = 1f;

    [SerializeField] float speed;

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
                //GetComponent<Rigidbody>().velocity = Vector3.zero;
                gObj.enabled = true;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
            //transform.position = Vector3.SmoothDamp(transform.position, player.transform.position, ref vel, maxRandomFactor);
        }
    }
}
