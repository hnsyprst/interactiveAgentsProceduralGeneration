using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPathAndFollow : MonoBehaviour
{
    public Transform Target;
    public float Speed = 5;

    Vector3[] Path;
    int TargetIndex;

    void Start()
    {
        // Send a pathfinding request to the Path Request Manager
        //PathRequestManager.RequestPath(transform.position, Target.position, OnPathFound);
    }

    public void OnPathFound(Vector3[] NewPath, bool Success)
    {
        if (Success)
        {
            Path = NewPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    public void RequestPath(Vector3 TargetPos)
    {
        // Send a pathfinding request to the Path Request Manager
        PathRequestManager.RequestPath(transform.position, TargetPos, OnPathFound);
    }

    IEnumerator FollowPath()
    {
        Vector3 CurrentWaypoint = Path[0];

        while (true)
        {
            if (transform.position == CurrentWaypoint)
            {
                TargetIndex++;
                // If TargetIndex is greater or equal to the number of waypoints in the path
                // The path must be finished, so exit the coroutine
                if (TargetIndex >= Path.Length)
                {
                    TargetIndex = 0;
                    Path = new Vector3[0];
                    yield break;
                }
                CurrentWaypoint = Path[TargetIndex];
            }

            // Actually steer towards the next waypoint
            transform.position = Vector3.MoveTowards(transform.position, CurrentWaypoint, Speed * Time.deltaTime);
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if (Path != null)
        {
            for (int i = TargetIndex; i < Path.Length; i++)
            {
                Gizmos.color = Color.cyan;
                Vector3 onezeroone = new Vector3(1, 0.01f, 1);
                Gizmos.DrawCube(Path[i], onezeroone);

                if (i == TargetIndex)
                {
                    Gizmos.DrawLine(transform.position, Path[i]);
                }
                else
                {
                    Gizmos.DrawLine(Path[i-1], Path[i]);
                }
            }
        }
    }
}
