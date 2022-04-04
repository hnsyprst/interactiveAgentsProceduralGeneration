using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceGenerator
{
    List<RoomSPNode> AllRoomNodes = new List<RoomSPNode>();
    int MapWidth, MapLength;

    public SpaceGenerator(int _MapWidth, int _MapLength)
    {
        MapWidth = _MapWidth;
        MapLength = _MapLength;
    }

    public List<RoomSPNode> CalculateRooms(int MaxIterations, int RoomWidthMin, int RoomLengthMin)
    {
        // Call the Binary Space Partitioner and split the space up into rooms
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(MapWidth, MapLength);
        AllRoomNodes = bsp.PrepareNodesCollection(MaxIterations, RoomWidthMin, RoomLengthMin);

        // The space has now been divided up into spaces, and sub-spaces, and sub-sub-spaces, and sub-sub-sub-spaces...
        // So we need to get all of the rooms that are undivided - the subbest of the sub-spaces
        // To do this we will traverse the graph (using each SPNode's ChildrenNodeList) to find the leaves
        List<RoomSPNode> DividedSpaces = ExtractLeavesFromGraph(bsp.RootRoomNode);

        return new List<RoomSPNode>(DividedSpaces);
    }

    public List<RoomSPNode> ExtractLeavesFromGraph(RoomSPNode ParentNode)
    {
        Queue<RoomSPNode> NodesToCheck = new Queue<RoomSPNode>();
        List<RoomSPNode> LeavesList = new List<RoomSPNode>();

        // If the node being examined has no children, it is a leaf! So just return this one node
        if (ParentNode.ChildrenNodeList.Count == 0)
        {
            LeavesList.Add(ParentNode);
            return LeavesList;
        }


        // Otherwise, add each child of the parent node to the queue
        foreach (RoomSPNode child in ParentNode.ChildrenNodeList)
        {
            NodesToCheck.Enqueue(child);
        }

        // While there are still items in the queue
        while (NodesToCheck.Count > 0)
        {
            // Get the first item in the queue
            RoomSPNode CurrentNode = NodesToCheck.Dequeue();

            // If this item has no children, it is a leaf! So add it to the list
            if (CurrentNode.ChildrenNodeList.Count == 0)
            {
                LeavesList.Add(CurrentNode);
            }
            // Otherwise, get all the children of this item and add them to the queue
            else
            {
                foreach (RoomSPNode child in CurrentNode.ChildrenNodeList)
                {
                    NodesToCheck.Enqueue(child);
                }
            }
        }

        return LeavesList;
    }
}
