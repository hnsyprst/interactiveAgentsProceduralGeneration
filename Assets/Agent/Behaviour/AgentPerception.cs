using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NPBehave;

public class AgentPerception : MonoBehaviour
{
    public LayerMask ResourceTileLayer;
    public float MaxPerceptionDistance;

    // These HashSets will store the locations of resource gameobjects
    // HashSets are used over lists so that the same resource can't be added twice
    public HashSet<Vector3> FoodLocations;
    public HashSet<Vector3> WaterLocations;
    public HashSet<Vector3> WoodLocations;

    Vector3[] PerceptionDirections;
    RaycastHit[] Hits;

    private void Awake()
    {
        FoodLocations = new HashSet<Vector3>();
        WaterLocations = new HashSet<Vector3>();
        WoodLocations = new HashSet<Vector3>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Directions in which the agent can see objects
        // Agents do not currently rotate, so they can see omnidirectionally for now
        PerceptionDirections = new Vector3[]
        {
            Vector3.forward,
            Vector3.right,
            Vector3.right+Vector3.forward,
            Vector3.left+Vector3.forward,
            Vector3.left,
            Vector3.left+Vector3.back,
            Vector3.right+Vector3.back,
            Vector3.back
        };
    }

    // Update is called once per frame
    void Update()
    {
        /*FoodLocations.Add(new Vector3(-21.5f, 0.2f, -16.5f));
        WaterLocations.Add(new Vector3(-8.5f, 0.2f, -20.5f));
        WoodLocations.Add(new Vector3(-1.5f, 0.2f, 11.5f));*/

        Hits = new RaycastHit[PerceptionDirections.Length];

        for (int i = 0; i < PerceptionDirections.Length; i++)
        {
            // Create a Vector3 pointing in this iteration's direction
            Vector3 Direction = transform.TransformDirection(PerceptionDirections[i]);

            // Perform a raycast in the direction just defined from the agent's current position to the maximum maximum distance this agent can percieve objects and store it in array 'hits'
            Physics.Raycast(transform.position, Direction, out Hits[i], MaxPerceptionDistance, ResourceTileLayer);

            // If the raycast hit a collider, draw a green line in this iteration's direction the length of the distance to the collider
            // In other words, if an object was in the path of the raycast, draw a green line to it
            if (Hits[i].collider != null)
            {
                Debug.DrawRay(transform.position, Direction * Hits[i].distance, Color.green);
            }
            // If the raycast did not hit a collider, draw a red line in this iteration's direction the maximum distance this agent can percieve objects
            else
            {
                Debug.DrawRay(transform.position, Direction * MaxPerceptionDistance, Color.red);
            }
        }

        // Discard any raycasts that did not hit
        Hits = Hits.ToList().Where(h => h.collider != null).ToArray();

        // If there are any collisions in the new list
        if (Hits.Length > 0)
        {
            foreach (RaycastHit hit in Hits)
            {
                // Check the tag of the hit gameobject
                if (hit.transform.tag == "Food")
                {
                    // If we hit a Food object, find its Interaction Target and add its position to the hash set
                    Vector3 InteractionTarget = hit.transform.Find("InteractionTarget").position;

                    FoodLocations.Add(InteractionTarget);
                }
                else if (hit.transform.tag == "Water")
                {
                    // Water objects are a special case - they have multiple Interaction Targets, one either side of the riverbank
                    // This is so that agents don't get stuck
                    // We have to add each of these to the WaterLocations hash set
                    //Vector3 InteractionTarget = hit.transform.Find("InteractionTarget").position;

                    foreach (Transform child in hit.transform)
                    {
                        if (child.name == "InteractionTarget")
                        {
                            WaterLocations.Add(child.transform.position);
                        }
                    }
                }
                else if (hit.transform.tag == "Wood")
                {
                    Vector3 InteractionTarget = hit.transform.Find("InteractionTarget").position;

                    WoodLocations.Add(InteractionTarget);
                }
            }
        }
    }
}
