using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tessera;

public class MapGenerator : MonoBehaviour
{
    public bool DisplayGizmos = true;

    public SpacePartitioner MapSpacePartitioner;
    public GameObject PathfindingGameObject;
    public Grid PathfindingGrid;

    public Vector3Int TileSize;

    public List<TileEntry> BushesAreaTiles;
    int BushesAreaCount = 0;
    public List<TileEntry> WaterAreaTiles;
    int WaterAreaCount = 0;
    public List<TileEntry> TreesAreaTiles;
    int TreesAreaCount = 0;
    public List<TileEntry> RuinsAreaTiles;
    int RuinsAreaCount = 0;

    private void Awake()
    {
        MapSpacePartitioner = GetComponent<SpacePartitioner>();
        PathfindingGrid = PathfindingGameObject.GetComponent<Grid>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Partition the map space, generating rooms for each partition
        List<RoomSPNode> RoomList = MapSpacePartitioner.PartitionSpace();

        // For each generated room, create a new wave funciton collapse (WFC) generator
        foreach (RoomSPNode room in RoomList)
        {
            TesseraGenerator WFCGenerator = gameObject.AddComponent<TesseraGenerator>();
            // If no bushes areas have been generated yet, generate one
            if (BushesAreaCount == 0)
            {
                Debug.Log("Generating Bush Area");
                WFCGenerator.tiles = BushesAreaTiles;
                BushesAreaCount++;
            }
            // If no water areas have been generated yet, generate one
            else if (WaterAreaCount == 0)
            {
                Debug.Log("Generating Water Area");
                WFCGenerator.tiles = WaterAreaTiles;
                WaterAreaCount++;
            }
            // If no trees areas have been generated yet, generate one
            else if (TreesAreaCount == 0)
            {
                Debug.Log("Generating Trees Area");
                WFCGenerator.tiles = TreesAreaTiles;
                TreesAreaCount++;
            }
            // If no ruins areas have been generated yet, generate one
            else if (RuinsAreaCount == 0)
            {
                Debug.Log("Generating ruins Area");
                WFCGenerator.tiles = RuinsAreaTiles;
                RuinsAreaCount++;
            }
            // Otherwise, pick a random area and generate that
            else
            {
                Debug.Log("Generating Random Area (bush)");
                WFCGenerator.tiles = BushesAreaTiles;
            }

            // Set up the remainder of the WFC generator settings
            WFCGenerator.tileSize = TileSize;

            // Set the WFC generator's size and position to the current room's size and position
            Vector3Int RoomSize = Vector3Int.FloorToInt(MapSpacePartitioner.GetRoomSize(room));
            Vector3 RoomCentre = MapSpacePartitioner.GetRoomCentre(room);
            // The WFC generator calculates its total size as a multiple of its tile size, so we divide by the tile size here
            WFCGenerator.size = new Vector3Int(RoomSize.x / TileSize.x, 1, RoomSize.z / TileSize.z);
            // We also divde centre by tile size so that generated areas fit nicely together

            /*int RoundedRoomCentreX = (int)Mathf.Round(RoomCentre.x / 5.0f) * 5;
            int RoundedRoomCentreZ = (int)Mathf.Round(RoomCentre.z / 5.0f) * 5;*/

            WFCGenerator.center = new Vector3(RoomCentre.x, 0, RoomCentre.z) + transform.position;

            // Generate an area
            WFCGenerator.backtrack = true;
            WFCGenerator.Generate();
        }

        PathfindingGrid.CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        MapSpacePartitioner.DisplayGizmos = DisplayGizmos;
    }
}
