using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steer : MonoBehaviour
{
    public bool SetWander = false;
    public Wander WanderScript;
    public float MovementSpeed = 2f;

    Vector3 Velocity;
    Vector3 WanderForce;

    // Start is called before the first frame update
    void Start()
    {
        Velocity = Vector3.zero;
        WanderForce = Vector3.zero;

        WanderScript = GetComponent<Wander>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SetWander)
        {
            WanderForce = WanderScript.GetWanderForce(Velocity);
        }

        // Actually steer towards the next waypoint
        transform.position += Velocity * MovementSpeed;
    }
}
