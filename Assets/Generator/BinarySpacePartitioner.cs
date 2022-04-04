using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinarySpacePartitioner
{
    public RoomSPNode RootRoomNode;

    public BinarySpacePartitioner(int _MapWidth, int _MapLength)
    {
        // Create the root node - it will spawn at 0,0 and will be the size passed to the constructor
        // As this node is the root, its parent node is null and its index is 0
        RootRoomNode = new RoomSPNode(new Vector2(0, 0), new Vector2(_MapWidth, _MapLength), null, 0);
    }
    
    public List<RoomSPNode> PrepareNodesCollection(int MaxIterations, int RoomWidthMin, int RoomLengthMin)
    {
        Queue<RoomSPNode> NodeGraph = new Queue<RoomSPNode>();
        List<RoomSPNode> NodeList = new List<RoomSPNode>();

        // NodeGraph stores the nodes to be split
        NodeGraph.Enqueue(RootRoomNode);
        // NodeList stores the split nodes
        NodeList.Add(RootRoomNode);

        int Iterations = 0;
        // While there are still iterations to be done and nodes left to be split
        while (Iterations < MaxIterations && NodeGraph.Count > 0)
        {
            Iterations++;
            RoomSPNode CurrentNode = NodeGraph.Dequeue();

            if (CurrentNode.Width >= RoomWidthMin * 2 || CurrentNode.Length >= RoomLengthMin * 2)
            {
                SplitSpace(CurrentNode, NodeList, RoomWidthMin, RoomLengthMin, NodeGraph);
            }
        }

        return NodeList;
    }

    void SplitSpace(RoomSPNode CurrentNode,
                    List<RoomSPNode> NodeList,
                    int RoomWidthMin,
                    int RoomLengthMin,
                    Queue<RoomSPNode> NodeGraph)
    {
        SPLine DividingLine = GetLineDividingSpace(CurrentNode.BottomLeftAreaCorner,
                                                   CurrentNode.TopRightAreaCorner,
                                                   RoomWidthMin,
                                                   RoomLengthMin);

        RoomSPNode NodeA, NodeB;

        if (DividingLine.Orientation == LineOrientation.Horizontal)
        {
            NodeA = new RoomSPNode(CurrentNode.BottomLeftAreaCorner,
                                   new Vector2(CurrentNode.TopRightAreaCorner.x, DividingLine.Coordinates.y),
                                   CurrentNode,
                                   CurrentNode.TreeIndex + 1);

            NodeB = new RoomSPNode(new Vector2(CurrentNode.BottomLeftAreaCorner.x, DividingLine.Coordinates.y),
                                   CurrentNode.TopRightAreaCorner,
                                   CurrentNode,
                                   CurrentNode.TreeIndex + 1);
        }
        else
        {
            NodeA = new RoomSPNode(CurrentNode.BottomLeftAreaCorner,
                                   new Vector2(DividingLine.Coordinates.x, CurrentNode.TopRightAreaCorner.y),
                                   CurrentNode,
                                   CurrentNode.TreeIndex + 1);

            NodeB = new RoomSPNode(new Vector2(DividingLine.Coordinates.x, CurrentNode.BottomLeftAreaCorner.y),
                                   CurrentNode.TopRightAreaCorner,
                                   CurrentNode,
                                   CurrentNode.TreeIndex + 1);
        }

        AddNewNodesToCollections(NodeList, NodeGraph, NodeA);
        AddNewNodesToCollections(NodeList, NodeGraph, NodeB);
    }

    void AddNewNodesToCollections(List<RoomSPNode> NodeList, Queue<RoomSPNode> NodeGraph, RoomSPNode Node)
    {
        NodeList.Add(Node);
        NodeGraph.Enqueue(Node);
    }

    SPLine GetLineDividingSpace(Vector2 BottomLeftAreaCorner, Vector2 TopRightAreaCorner, int RoomWidthMin, int RoomLengthMin)
    {
        LineOrientation Orientation;
        // Calculate whether the room is valid for splitting across its length and width
        // according to the minimum room length and width passed to this method
        bool LengthIsValid = (TopRightAreaCorner.y - BottomLeftAreaCorner.y) >= 2 * RoomLengthMin;
        bool WidthIsValid = (TopRightAreaCorner.x - BottomLeftAreaCorner.x) >= 2 * RoomWidthMin;

        // If the space is valid for splitting across both its length and width
        if (LengthIsValid && WidthIsValid)
        {
            // Randomly create either a horizontal or vertical split across the space using the LineOrientation enum
            Orientation = (LineOrientation)(Random.Range(0, 2));
        }
        // If the space is only valid for splitting across its width
        else if (WidthIsValid)
        {
            // Split the space across its width (i.e., split it vertically)
            Orientation = LineOrientation.Vertical;
        }
        // If the space is only valid for splitting across its length
        else
        {
            // Split the space across its length (i.e., split it horizontally)
            Orientation = LineOrientation.Horizontal;
        }

        // Generate a new dividing line, its orientation chosen above, and its coordinates randomly chosen based on both the 
        // min and max bounds set by RoomWidthMin and RoomLengthMin and the orientation of the room
        return new SPLine(Orientation, GetCoordinatesForOrientation(Orientation,
                                                                    BottomLeftAreaCorner,
                                                                    TopRightAreaCorner,
                                                                    RoomWidthMin,
                                                                    RoomLengthMin));
    }

    Vector2 GetCoordinatesForOrientation(LineOrientation Orientation,
                                         Vector2 BottomLeftAreaCorner,
                                         Vector2 TopRightAreaCorner,
                                         int RoomWidthMin,
                                         int RoomLengthMin)
    {
        Vector2 Coordinates = Vector2.zero;

        // If the room is going to be split vertically, generate a valid random position for the new vertical dividing line
        if (Orientation == LineOrientation.Vertical)
        {
            // These variables ensure that the line we use to split a room is valid (i.e., not outside the min and max bounds we set)
            float MinimumRoomDivisionWidth = BottomLeftAreaCorner.x + RoomWidthMin;
            float MaximumRoomDivisionWidth = TopRightAreaCorner.x - RoomWidthMin;
            Coordinates = new Vector2(Random.Range(MinimumRoomDivisionWidth, MaximumRoomDivisionWidth), 0);
        }
        // If the room is going to be split horizontally, generate a valid random position for the new horizontal dividing line
        else
        {
            // These variables ensure that the line we use to split a room is valid (i.e., not outside the min and max bounds we set)
            float MinimumRoomDivisionLength = BottomLeftAreaCorner.y + RoomLengthMin;
            float MaximumRoomDivisionLength = TopRightAreaCorner.y - RoomLengthMin;
            Coordinates = new Vector2(0, Random.Range(MinimumRoomDivisionLength, MaximumRoomDivisionLength));
        }

        int CoordinateRoundedX = (int)Mathf.Round(Coordinates.x / 5.0f) * 5;
        int CoordinateRoundedY = (int)Mathf.Round(Coordinates.y / 5.0f) * 5;

        Coordinates = new Vector2(CoordinateRoundedX, CoordinateRoundedY);

        return Coordinates;
    }
}
