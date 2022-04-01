using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To make this item storable on a heap, we must implement the methods required by IHeapItem
public class Node : IHeapItem<Node>
{
    public Node Parent;
    public Vector3 WorldPos;
    public bool IsWalkable;

    public int GridX;
    public int GridY;

    public int GCost;
    public int HCost;
    public int FCost
    {
        get
        {
            return GCost + HCost;
        }
    }

    int MyHeapIndex;

    public Node(Vector3 _WorldPos, bool _IsWalkable, int _GridX, int _GridY)
    {
        WorldPos = _WorldPos;
        IsWalkable = _IsWalkable;

        GridX = _GridX;
        GridY = _GridY;
    }

    public int HeapIndex
    {
        get
        {
            return MyHeapIndex;
        }
        set
        {
            MyHeapIndex = value;
        }
    }

    public int CompareTo(Node NodeToCompare)
    {
        // Compare the FCosts (total costs) of this node and the NodeToCompare
        int Comparison = FCost.CompareTo(NodeToCompare.FCost);
        // If they are equal, compare HCosts (heuristic costs) instead as a tie-breaker
        if (Comparison == 0)
        {
            Comparison = HCost.CompareTo(NodeToCompare.HCost);
        }
        // In pathfinding, the node with the LOWER cost is higher priority
        // So we invert the result of CompareTo
        return -Comparison;
    }
}
