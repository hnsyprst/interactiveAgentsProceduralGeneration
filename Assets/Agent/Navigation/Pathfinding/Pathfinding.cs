using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform Start, Target;

    PathRequestManager RequestManager;
    Grid NodeGrid;

    void Awake()
    {
        RequestManager = GetComponent<PathRequestManager>();
        NodeGrid = GetComponent<Grid>();
    }

    void Update()
    {

    }

    public void StartFindPath(Vector3 StartPos, Vector3 TargetPos)
    {
        StartCoroutine(FindPath(StartPos, TargetPos));
    }

    IEnumerator FindPath(Vector3 StartPos, Vector3 TargetPos)
    {
        // This variable will store the final path in world space
        // Agents can steer their own routes along the path using this array
        Vector3[] Waypoints = new Vector3[0];
        // This variable will only be true if Pathfinding can actually find a path
        bool PathfindingSuccess = false;

        // Convert world positions to nodes in the grid of nodes
        PFNode StartNode = NodeGrid.GetNodeFromWorldPos(StartPos);
        PFNode TargetNode = NodeGrid.GetNodeFromWorldPos(TargetPos);

        // Only actually search for a path if both start and target nodes are valid
        if (StartNode.IsWalkable && TargetNode.IsWalkable)
        {
            // Create data structures for storing explorable and explored nodes
            Heap<PFNode> Frontier = new Heap<PFNode>(NodeGrid.MaxSize);
            HashSet<PFNode> Explored = new HashSet<PFNode>();

            Frontier.Add(StartNode);

            // Expand nodes in the Frontier while there are still nodes to expand
            while (Frontier.Count > 0)
            {
                PFNode CurrentNode = Frontier.Pop();
                Explored.Add(CurrentNode);

                // If the current node is the target node, we have arrived! Exit the loop
                if (CurrentNode == TargetNode)
                {
                    PathfindingSuccess = true;
                    break;
                }

                // Expand the current node's neighbours
                foreach (PFNode neighbour in NodeGrid.GetNeighboursFromNode(CurrentNode))
                {
                    // If this neighbour is not walkable, or has already been explored, ignore this neighbour
                    if (!neighbour.IsWalkable || Explored.Contains(neighbour))
                    {
                        continue;
                    }

                    int DistanceCostToNeighbour = CurrentNode.GCost + GetDistance(CurrentNode, neighbour);
                    // If the distance cost to get to this neighbour is less than the current node's distance cost
                    // Or the neighbour is not in the frontier
                    // Add distance cost, heursitic cost and parent properties to the neighbour node
                    // And add the neighbour to the Frontier
                    if (DistanceCostToNeighbour < neighbour.GCost || !Frontier.Contains(neighbour))
                    {
                        neighbour.GCost = DistanceCostToNeighbour;
                        neighbour.HCost = GetDistance(neighbour, TargetNode);
                        neighbour.Parent = CurrentNode;
                        if (!Frontier.Contains(neighbour))
                        {
                            Frontier.Add(neighbour);
                        }
                        else
                        {
                            Frontier.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }
        
        yield return null;
        // If Pathfinding was successful, reconstruct the path and store it in Waypoints
        if (PathfindingSuccess)
        {
            Waypoints = ReconstructPath(StartNode, TargetNode);
        }
        RequestManager.FinishedProcessingPath(Waypoints, PathfindingSuccess);
    }

    Vector3[] ReconstructPath(PFNode _StartNode, PFNode _TargetNode)
    {
        List<PFNode> Path = new List<PFNode>();
        // We trace the path backwards so that we can access Nodes' Parent properties
        PFNode CurrentNode = _TargetNode;

        while (CurrentNode != _StartNode)
        {
            Path.Add(CurrentNode);
            CurrentNode = CurrentNode.Parent;
        }

        Vector3[] Waypoints = SimplifyPath(Path);
        Array.Reverse(Waypoints);
        return Waypoints;
    }

    Vector3[] SimplifyPath(List<PFNode> Path)
    {
        List<Vector3> Waypoints = new List<Vector3>();
        Vector2 DirectionOld = Vector2.zero;

        for (int i = 1; i < Path.Count; i++)
        {
            Vector2 DirectionNew = new Vector2(Path[i - 1].GridX - Path[i].GridX, Path[i - 1].GridY - Path[i].GridY);
            // Only add a new waypoint if the path changes direction
            if (DirectionNew != DirectionOld)
            {
                Vector3 WaypointPos = Path[i - 1].WorldPos;
                Vector3 WaypointNoY = new Vector3(WaypointPos.x, 1.0f, WaypointPos.z);
                Waypoints.Add(WaypointNoY);
            }
            DirectionOld = DirectionNew;
        }
        return Waypoints.ToArray();
    }

    // For the distance calculation, Nodes are taken to spaced out by a 'distance' of 10
    // This means that the horizontal or vertical distance between any 2 neighbouring nodes is 10
    // We can therefore calculate the diagonal distance using Pythagoras' theorem:
    // Diagonal distance = sqrt(10^2 + 10^2) = approximately 14
    int GetDistance(PFNode NodeA, PFNode NodeB)
    {
        // Get the distance JUST along the X axis between NodeA and NodeB
        int DistanceX = Mathf.Abs(NodeA.GridX - NodeB.GridX);
        // Get the distance JUST along the Y axis between NodeA and NodeB
        int DistanceY = Mathf.Abs(NodeA.GridY - NodeB.GridY);

        if (DistanceX > DistanceY)
        {
            // If the distance along the X axis is greater than along the Y,
            // We first move diagonally for Y distance to position ourselves
            // in line with NodeB along the Y axis, and then move the remainder
            // (our diagonal move will have moved us along the X axis, too)
            // of the X distance along the X axis
            return 14 * DistanceY + 10 * (DistanceX - DistanceY);
        }
        else
        {
            // If the distance along the Y axis is greater than along the X,
            // We first move diagonally for X distance to position ourselves
            // in line with NodeB along the Y axis, and then move the remainder
            // (our diagonal move will have moved us along the Y axis, too)
            // of the Y distance along the Y axis
            return 14 * DistanceX + 10 * (DistanceY - DistanceX);
        }
    }
}
