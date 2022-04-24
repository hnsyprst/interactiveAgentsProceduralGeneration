using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steer : MonoBehaviour
{
    public Rigidbody RB;
    public bool SetWander = false;
    public Wander WanderScript;
    public float MovementSpeed = 0.1f;
    public float MaximumForce = 2f;

    Vector3 Velocity;
    Vector3 WanderForce;

    // Start is called before the first frame update
    void Start()
    {
        Velocity = Vector3.zero;
        WanderForce = Vector3.zero;

        WanderScript = GetComponent<Wander>();
        RB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (SetWander)
        {
            WanderForce = WanderScript.GetWanderForce(Velocity);
        }
        else
        {
            Velocity = Vector3.zero;
            WanderForce = Vector3.zero;
        }

        Vector3 DesiredVelocity = WanderForce.normalized * MovementSpeed;
        Vector3 SteeringForce = DesiredVelocity - Velocity;
        SteeringForce = Vector3.ClampMagnitude(SteeringForce, MaximumForce);

        Velocity = Vector3.ClampMagnitude(Velocity + WanderForce, MaximumForce);

        // Actually steer towards the next waypoint
        transform.position += Velocity * Time.deltaTime;
        transform.forward = Velocity.normalized;
    }
}
