using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : MonoBehaviour
{
    public float CircleDistance = 2f;
    public float CircleRadius = 2f;

    public Vector3 GetWanderForce(Vector3 CurrentVelocity)
    {
        Vector3 CircleCentre = CurrentVelocity.normalized;
        Vector2 RandomCirclePoint = Random.insideUnitCircle.normalized;

        Vector3 Displacement = new Vector3(RandomCirclePoint.x, 0, RandomCirclePoint.y);
        Displacement *= CircleRadius;
        Displacement = Quaternion.LookRotation(CurrentVelocity) * Displacement;

        return CircleCentre + Displacement;
    }
}
