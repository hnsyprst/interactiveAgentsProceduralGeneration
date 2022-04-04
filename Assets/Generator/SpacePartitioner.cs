using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacePartitioner : MonoBehaviour
{
    public int MapWidth, MapLength;
    public int RoomWidthMin, RoomLengthMin;
    public int MaxIterations;
    public int CorridorWidth;

    List<RoomSPNode> RoomList = new List<RoomSPNode>();

    List<Color> Colors = new List<Color>();

    public bool DisplayGizmos = true;

    // Start is called before the first frame update
    void Start()
    {
        //PartitionSpace();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<RoomSPNode> PartitionSpace()
    {
        SpaceGenerator Generator = new SpaceGenerator(MapWidth, MapLength);
        RoomList = Generator.CalculateRooms(MaxIterations, RoomWidthMin, RoomLengthMin);
        for (int i = 0; i < RoomList.Count; i++)
        {
            Colors.Add(Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
        }

        return RoomList;
    }

    public Vector3 GetRoomCentre(RoomSPNode Room)
    {
        float CentreX = (Room.BottomLeftAreaCorner.x + Room.BottomRightAreaCorner.x) / 2;
        float CentreY = (Room.TopLeftAreaCorner.y + Room.BottomLeftAreaCorner.y) / 2;
        Vector3 CentrePoint = new Vector3(CentreX, 0, CentreY);

        CentrePoint.x -= MapWidth / 2;
        CentrePoint.z -= MapLength / 2;

        return CentrePoint;
    }

    public Vector3 GetRoomSize(RoomSPNode Room)
    {
        float LengthX = Room.BottomRightAreaCorner.x - Room.BottomLeftAreaCorner.x;
        float LengthY = Room.TopLeftAreaCorner.y - Room.BottomLeftAreaCorner.y;
        Vector3 Size = new Vector3(LengthX, 0, LengthY);

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