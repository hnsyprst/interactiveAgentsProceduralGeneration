using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPathAndFollow : MonoBehaviour
{
    public Transform Target;
    public float Speed = 5;

    Vector3[] Path;
    int TargetIndex;

    public bool IsFollowingPath;

    void Start()
    {
        // Send a pathfinding request to the Path Request Manager
        //PathRequestManager.RequestPath(transform.position, Target.position, OnPathFound);

        IsFollowingPath = false;
    }

    public void OnPathFound(Vector3[] NewPath, bool Success)
    {
        if (Success)
        {
            // Stop the coroutine and zero out the path
            // This stops agents from behaving strangely when their path suddenly changes
            StopCoroutine("FollowPath");
            TargetIndex = 0;
            Path = new Vector3[0];

            // Apply the new path
            Path = NewPath;

            // Start following the new path
            IsFollowingPath = true;
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
                    // Reset path following variables, ready for the next path
                    TargetIndex = 0;
                    Path = new Vector3[0];
                    IsFollowingPath = false;
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
                Vector3 onezeroone = new Vector3(1, 1f, 1);
                Gizmos.DrawCube(Path[i], onezeroone);

                if (i == TargetIndex)
                {
                    Gizmos.DrawLine(transform.position + new Vector3(0, 2, 0), Path[i]);
                }
                else
                {
                    Gizmos.DrawLine(Path[i-1], Path[i]);
                }
            }
        }
    }
}
