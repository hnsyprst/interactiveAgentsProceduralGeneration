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
        List<RoomSPNode> DividedSpaces = GraphHelper.ExtractLeavesFromGraph(bsp.RootRoomNode);

        //RoomGenerator roomGenerator = new RoomGenerator(MaxIterations, RoomWidthMin, RoomLengthMin);
        //List<RoomSPNode> RoomList = roomGenerator.GenerateRooms(DividedSpaces);
        return new List<RoomSPNode>(DividedSpaces);
    }
}
