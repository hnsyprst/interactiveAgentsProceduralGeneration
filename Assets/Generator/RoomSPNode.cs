using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSPNode : SPNode
{
    public float Width
    {
        get
        {
            return TopRightAreaCorner.x - BottomLeftAreaCorner.x;
        }
    }

    public float Length
    {
        get
        {
            return TopRightAreaCorner.y - BottomLeftAreaCorner.y;
        }
    }

    public RoomSPNode(Vector2 _BottomLeftAreaCorner,
                      Vector2 _TopRightAreaCorner,
                      SPNode _ParentNode,
                      int _TreeIndex) : base(_ParentNode)
    {
        BottomLeftAreaCorner = _BottomLeftAreaCorner;
        TopRightAreaCorner = _TopRightAreaCorner;

        // Calculate the other coordinates from the coordinates passed in the constructor
        // This saves space when we create this class (less things to pass to the constructor!)
        BottomRightAreaCorner = new Vector2(TopRightAreaCorner.x, BottomLeftAreaCorner.y);
        TopLeftAreaCorner = new Vector2(BottomLeftAreaCorner.x, TopRightAreaCorner.y);

        TreeIndex = _TreeIndex;
    }
}
