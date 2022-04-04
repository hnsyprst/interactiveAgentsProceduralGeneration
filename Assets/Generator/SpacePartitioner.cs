using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacePartitioner : MonoBehaviour
{
    public int MapWidth, MapLength;
    public int RoomWidthMin, RoomLengthMin;
    public int MaxIterations;

    List<RoomSPNode> RoomList = new List<RoomSPNode>();

    List<Color> Colors = new List<Color>();

    public bool DisplayGizmos = true;

    public List<RoomSPNode> PartitionSpace()
    {
        // Partition the map size given by MapWidth and MapLength using SpaceGenerator
        SpaceGenerator Generator = new SpaceGenerator(MapWidth, MapLength);
        RoomList = Generator.CalculateRooms(MaxIterations, RoomWidthMin, RoomLengthMin);
        // For each generated room, generate a new colour to display it in for debug
        for (int i = 0; i < RoomList.Count; i++)
        {
            Colors.Add(Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
        }
        return RoomList;
    }

    public Vector3 GetRoomCentre(RoomSPNode Room)
    {
        // Calculate the centre point of the given room
        float CentreX = (Room.BottomLeftAreaCorner.x + Room.BottomRightAreaCorner.x) / 2;
        float CentreY = (Room.TopLeftAreaCorner.y + Room.BottomLeftAreaCorner.y) / 2;
        Vector3 CentrePoint = new Vector3(CentreX, 0, CentreY);

        CentrePoint.x -= MapWidth / 2;
        CentrePoint.z -= MapLength / 2;

        return CentrePoint;
    }

    public Vector3 GetRoomSize(RoomSPNode Room)
    {
        // Get the size of the given room
        Vector3 Size = new Vector3(Room.Width, 0, Room.Length);

        return Size;
    }

    void OnDrawGizmos()
    {
        // In Unity's 3D space, the Y axis represents 'Up' - so we'll pass MapLength to the Z axis here instead
        Gizmos.DrawWireCube(transform.position, new Vector3(MapWidth, 1, MapLength));

        if (DisplayGizmos)
        {
            // If the RoomList has actually been populated
            if (RoomList != null)
            {
                int Count = 0;
                foreach (RoomSPNode room in RoomList)
                {
                    Gizmos.color = Colors[Count];
                    Count++;

                    Vector3 CentrePoint = GetRoomCentre(room);
                    CentrePoint += transform.position;

                    Vector3 Size = GetRoomSize(room);

                    Gizmos.DrawCube(CentrePoint, Size);
                }
            }
        }
    }
}