using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool DisplayDebugGrid;
    public Transform Player;
    public Vector2 GridWorldSize;
    public float NodeRadius;
    public LayerMask UnwalkableMask;
    public int MaxSize
    {
        get
        {
            return GridSizeX * GridSizeY;
        }
    }

    // 2D array of nodes (our grid to pathfind on)
    PFNode[,] NodeGrid;

    float NodeDiameter;
    int GridSizeX, GridSizeY;

    void Awake()
    {
        NodeDiameter = NodeRadius * 2;

        // Calculate number of nodes that can fit inside the specified grid size along the X axis
        GridSizeX = Mathf.RoundToInt(GridWorldSize.x / NodeDiameter);
        // Calculate number of nodes that can fit inside the specified grid size along the Y axis
        GridSizeY = Mathf.RoundToInt(GridWorldSize.y / NodeDiameter);

        CreateGrid();
    }

    // Populate the grid of nodes
    void CreateGrid()
    {
        // Initialise the grid of nodes with the number of nodes calculated in Start()
        NodeGrid = new PFNode[GridSizeX, GridSizeY];

        // Calculate the bottom left position of the grid in world space
        Vector3 WorldLeftEdge = Vector3.right * GridWorldSize.x / 2;
        // (we use forward here because in Unity's 3D space, the Y axis represents 'Up', rather than Z representing 'Up')
        Vector3 WorldBottomEdge = Vector3.forward * GridWorldSize.y / 2;
        Vector3 WorldBottomLeft = transform.position - WorldLeftEdge - WorldBottomEdge;

        for (int x = 0; x < GridSizeX; x++)
        {
            for (int y = 0; y < GridSizeY; y++)
            {
                // Get the X and Y positions of the current node's centre point
                float CurrentNodeCentreX = x * NodeDiameter + NodeRadius;
                float CurrentNodeCentreY = y * NodeDiameter + NodeRadius;
                // Convert these X and Y positions into world space
                Vector3 WorldPos = WorldBottomLeft + Vector3.right * CurrentNodeCentreX + Vector3.forward * CurrentNodeCentreY;
                // Check that there is no obstacle at the current node's position in world space
                bool IsWalkable = !(Physics.CheckSphere(WorldPos, NodeRadius, UnwalkableMask));

                // Create a new node in our grid of nodes using these values
                NodeGrid[x, y] = new PFNode(WorldPos, IsWalkable, x, y);
            }
        }
    }

    public List<PFNode> GetNeighboursFromNode(PFNode node)
    {
        List<PFNode> Neighbours = new List<PFNode>();

        // Search in a 3x3 block around the node (diagram below)
        //  -1, -1  0, -1   1, -1
        //  -1, 0   node    1, 0
        //  -1, 1   0, 1    1, 1
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // If our coordinates are (0, 0), we are in the centre points (i.e., node's position)
                // (see diagram above), so we should skip this iteration
                if (x == 0 && y == 0) continue;

                // Calculate the position of this neighbour within the grid of nodes
                int NeighbourX = node.GridX + x;
                int NeighbourY = node.GridY + y;

                // Check that the neighbour is actually within the grid (helpful when node is at the edge of the grid)
                if (NeighbourX >= 0 && NeighbourX < GridSizeX && NeighbourY >= 0 && NeighbourY < GridSizeY)
                {
                    // If the neighbour is within the grid, get the node from the node grid and add it to the Neighbours list
                    Neighbours.Add(NodeGrid[NeighbourX, NeighbourY]);
                }
            }
        }

        return Neighbours;
    }

    public PFNode GetNodeFromWorldPos(Vector3 _WorldPos)
    {
        // Calculate how far along (by percentage) the passed world position is along the grid of nodes in world space
        float PercentX = (_WorldPos.x + GridWorldSize.x / 2) / GridWorldSize.x;
        float PercentY = (_WorldPos.z + GridWorldSize.y / 2) / GridWorldSize.y;

        // The percentage is clamped to between 0 and 1 so that if an invalid world position is passed errors 
        // arising from attempting to access the node grid out of range will be avoided
        PercentX = Mathf.Clamp01(PercentX);
        PercentY = Mathf.Clamp01(PercentY);

        // Get the index in the NodeGrid array corresponding to the calculated percentage
        // (we subtract 1 because arrays start at 0)
        int x = Mathf.RoundToInt((GridSizeX - 1) * PercentX);
        int y = Mathf.RoundToInt((GridSizeY - 1) * PercentY);

        return NodeGrid[x, y];
    }

    void OnDrawGizmos()
    {
        // In Unity's 3D space, the Y axis represents 'Up' - so we'll pass the Y coord from GridWorldSize to the Z axis here instead
        Gizmos.DrawWireCube(transform.position, new Vector3(GridWorldSize.x, 1, GridWorldSize.y));

        // If NodeGrid has actually been populated
        if (NodeGrid != null && DisplayDebugGrid)
        {
            // Draw gizmos for the pathfinding agents
            PFNode PlayerNode = GetNodeFromWorldPos(Player.position);

            // Draw gizmos for each node in the grid of nodes
            foreach (PFNode node in NodeGrid)
            {
                Color DrawColor = new Color();
                // If the node is walkable, draw it in white, otherwise draw it in red
                DrawColor = node.IsWalkable ? Color.white : Color.red;
                // If the node is where a pathfinding agent is standing, draw it in blue
                if (node == PlayerNode) DrawColor = Color.blue;

                DrawColor.a = 0.5f;
                Gizmos.color = DrawColor;

                // We want to draw our grid as (almost) squares, not cubes
                Vector3 onezeroone = new Vector3(1, 0.01f, 1);
                Vector3 UpwardTranslation = new Vector3(0, 0.2f, 0);
                Gizmos.DrawCube(node.WorldPos + UpwardTranslation, onezeroone * (NodeDiameter - 0.1f));
            }
        }
    }
}
