using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SPNode
{
    public Vector2 BottomLeftAreaCorner;
    public Vector2 BottomRightAreaCorner;
    public Vector2 TopLeftAreaCorner;
    public Vector2 TopRightAreaCorner;

    public int TreeIndex;

    public SPNode ParentNode;

    List<SPNode> MyChildrenNodeList;

    public List<SPNode> ChildrenNodeList
    {
        get
        {
            return MyChildrenNodeList;
        }
    }

    public SPNode(SPNode _ParentNode)
    {
        // When this node gets created, add this node to its parents' list of children
        MyChildrenNodeList = new List<SPNode>();
        ParentNode = _ParentNode;

        if (ParentNode != null)
        {
            ParentNode.AddChild(this);
        }
    }

    public void AddChild(SPNode Child)
    {
        MyChildrenNodeList.Add(Child);
    }
}