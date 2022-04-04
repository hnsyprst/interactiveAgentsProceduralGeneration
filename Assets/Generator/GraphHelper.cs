using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GraphHelper
{
    public static List<RoomSPNode> ExtractLeavesFromGraph(RoomSPNode ParentNode)
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

    public static Vector2 GenerateBottomLeftCornerBetween(Vector2 BottomLeftAreaCorner, Vector2 TopRightAreaCorner, float RoomSizeModifier, float Offset)
    {
        // Offset is optionally used to create negative spaces between areas
        float MinX = BottomLeftAreaCorner.x + Offset;
        float MaxX = TopRightAreaCorner.x - Offset;
        float MinY = BottomLeftAreaCorner.y + Offset;
        float MaxY = TopRightAreaCorner.y - Offset;

        // RoomSizeModifier is optionally used to add more variety to the room generation
        // A higher value means rooms will vary more in how much of their allocated space they will use up
        float ModifiedRoomX = MinX + (MaxX - MinX) * RoomSizeModifier;
        float ModifiedRoomY = MinY + (MaxY - MinY) * RoomSizeModifier;

        float RandomX = Random.Range(MinX, ModifiedRoomX);
        float RandomY = Random.Range(MinY, ModifiedRoomY);

        return new Vector2(RandomX, RandomY);
    }

    public static Vector2 GenerateTopRightCornerBetween(Vector2 BottomLeftAreaCorner, Vector2 TopRightAreaCorner, float RoomSizeModifier, float Offset)
    {
        // Offset is optionally used to create negative spaces between areas
        float MinX = BottomLeftAreaCorner.x + Offset;
        float MaxX = TopRightAreaCorner.x - Offset;
        float MinY = BottomLeftAreaCorner.y + Offset;
        float MaxY = TopRightAreaCorner.y - Offset;

        // RoomSizeModifier is optionally used to add more variety to the room generation
        // A higher value means rooms will vary more in how much of their allocated space they will use up
        float ModifiedRoomX = MinX + (MaxX - MinX) * RoomSizeModifier;
        float ModifiedRoomY = MinY + (MaxY - MinY) * RoomSizeModifier;

        float RandomX = Random.Range(ModifiedRoomX, MaxX);
        float RandomY = Random.Range(ModifiedRoomY, MaxY);

        return new Vector2(RandomX, RandomY);
    }
}
