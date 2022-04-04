using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator
{
    int MaxIterations;
    int RoomWidthMin, RoomLengthMin;

    public RoomGenerator(int _MaxIterations, int _RoomWidthMin, int _RoomLengthMin)
    {
        MaxIterations = _MaxIterations;
        RoomWidthMin = _RoomWidthMin;
        RoomLengthMin = _RoomLengthMin;
    }

    public List<RoomSPNode> GenerateRooms(List<SPNode> DividedSpaces)
    {
        List<RoomSPNode> RoomList = new List<RoomSPNode>();

        foreach (SPNode space in DividedSpaces)
        {
            Vector2 NewBottomLeftPoint = GraphHelper.GenerateBottomLeftCornerBetween(space.BottomLeftAreaCorner, space.TopRightAreaCorner, 0.1f, 1f);
            Vector2 NewTopRightPoint = GraphHelper.GenerateTopRightCornerBetween(space.BottomLeftAreaCorner, space.TopRightAreaCorner, 0.9f, 1f);

            space.BottomLeftAreaCorner = NewBottomLeftPoint;
            space.BottomRightAreaCorner = new Vector2(NewTopRightPoint.x, NewBottomLeftPoint.y);
            space.TopRightAreaCorner = NewTopRightPoint;
            space.TopLeftAreaCorner = new Vector2(NewBottomLeftPoint.x, NewTopRightPoint.y);

            // Cast space to RoomSPNode and add it to the list
            RoomList.Add((RoomSPNode)space);
        }

        return RoomList;
    }
}
