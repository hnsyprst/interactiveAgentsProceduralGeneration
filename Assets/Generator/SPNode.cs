using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SPNode
{
    public bool Visited;

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

    public void RemoveChild(SPNode Child)
    {
        MyChildrenNodeList.Remove(Child);
    }
}