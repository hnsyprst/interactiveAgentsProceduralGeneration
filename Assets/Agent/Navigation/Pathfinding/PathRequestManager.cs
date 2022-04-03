using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    // A queue to store pathfinding requests
    Queue<PathRequest> PathRequestQueue = new Queue<PathRequest>();
    PathRequest CurrentPathRequest;

    // We only ever need one PathRequestManager
    static PathRequestManager instance;
    public static Pathfinding pathfinding;
    bool IsProcessingPath;

    // Data structure for storing the details of pathfinding requests
    struct PathRequest
    {
        public Vector3 PathStart;
        public Vector3 PathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _PathStart, Vector3 _PathEnd, Action<Vector3[], bool> _callback)
        {
            PathStart = _PathStart;
            PathEnd = _PathEnd;
            callback = _callback;
        }
    }

    private void Awake()
    {
        pathfinding = GetComponent<Pathfinding>();
        // Set the current PathRequestManager to this PathRequestManager
        instance = this;
    }

    public static void RequestPath(Vector3 PathStart, Vector3 PathEnd, Action<Vector3[], bool> callback)
    {
        // Create a new PathRequest
        PathRequest NewRequest = new PathRequest(PathStart, PathEnd, callback);
        // Add it to the PathRequestQueue
        instance.PathRequestQueue.Enqueue(NewRequest);
        // Process the next PathRequest in the queue if possible
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        // If not currently processing a path, and the path request queue is not empty
        if (!IsProcessingPath && PathRequestQueue.Count > 0)
        {
            CurrentPathRequest = PathRequestQueue.Dequeue();
            IsProcessingPath = true;
            pathfinding.StartFindPath(CurrentPathRequest.PathStart, CurrentPathRequest.PathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3[] ProcessedPath, bool Success)
    {
        // Pass the processed path to the callback function passed to CurrentPathRequest
        CurrentPathRequest.callback(ProcessedPath, Success);
        IsProcessingPath = false;
        // Process the next PathRequest in the queue if possible
        instance.TryProcessNext();
    }
}
