using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NPBehave;

public class AgentBT : MonoBehaviour
{
    // Reference to agent's path request and following script
    public GetPathAndFollow AgentPathfinding;
    public Steer AgentSteering;

    // Reference to agent's status (hunger, thirst)
    public AgentStatus CurrentAgentStatus;

    // Reference to agent's communication script
    public AgentCommunicate CurrentAgentCommunicate;

    // Reference to agent's perception script
    public AgentPerception CurrentAgentPerception;

    // References to lists of interactable objects in the world
    HashSet<Vector3> FoodLocations, WaterLocations, WoodLocations;

    Root bt_Tree;
    Blackboard bt_Blackboard;

    // List of behaviour trees
    const int AGENT_IDLE = 0;
    const int AGENT_DRINK = 1;
    const int AGENT_EAT = 2;
    const int AGENT_CHOP_WOOD = 3;

    int CurrentAction;
    public List<float> UtilityScores;

    bool IsFollowingPath;

    // Start is called before the first frame update
    void Start()
    {
        AgentPathfinding = GetComponent<GetPathAndFollow>();
        AgentSteering = GetComponent<Steer>();
        CurrentAgentStatus = GetComponent<AgentStatus>();
        CurrentAgentCommunicate = GetComponentInChildren<AgentCommunicate>();
        CurrentAgentPerception = GetComponentInChildren<AgentPerception>();

        // Set initial action
        CurrentAction = AGENT_IDLE;
        SwitchTree(SelectBehaviourTree(CurrentAction));

        // Set utility scores to zero
        UtilityScores = new List<float>();
        UtilityScores.Add(0); // AGENT_IDLE
        UtilityScores.Add(0); // AGENT_DRINK
        UtilityScores.Add(0); // AGENT_EAT
        UtilityScores.Add(0); // AGENT_CHOP_WOOD
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePerception();
        UpdateScores();
        IsFollowingPath = AgentPathfinding.IsFollowingPath;
        float maxValue = UtilityScores.Max(t => t);
        int maxIndex = UtilityScores.IndexOf(maxValue);

        if (CurrentAction != maxIndex)
        {
            CurrentAction = maxIndex;
            SwitchTree(SelectBehaviourTree(CurrentAction));
        }
    }

    void UpdateScores()
    {
        UtilityScores[AGENT_IDLE] = 0;
        UtilityScores[AGENT_DRINK] = CurrentAgentStatus.Thirst;
        UtilityScores[AGENT_EAT] = CurrentAgentStatus.Hunger;
        UtilityScores[AGENT_CHOP_WOOD] = 0;
    }

    void UpdatePerception()
    {
        FoodLocations = CurrentAgentPerception.FoodLocations;
        WaterLocations = CurrentAgentPerception.WaterLocations;
        WoodLocations = CurrentAgentPerception.WoodLocations;
    }

    Vector3 GetClosest(HashSet<Vector3> ResourceLocations)
    {
        Vector3 ClosestPosition = Vector3.zero;
        // Start out with 'infinite' distance
        int Distance = 2147483647;

        // For each position of this resource that the agent knows about
        foreach (Vector3 position in ResourceLocations)
        {
            // Use the Pathfinding script's GetDistance function to calculate the distance 
            // from the agent's current position to the resource position
            int NewDistance = PathRequestManager.pathfinding.GetDistance(transform.position, position);
            // If that distance is lower than the lowest distance found so far, set ClosestPosition
            // to the current resource position
            if (NewDistance < Distance)
            {
                ClosestPosition = position;
            }
        }

        return ClosestPosition;
    }

    void SwitchTree(Root _bt_Tree)
    {
        // Stop the currently running tree (if there is one)
        if (bt_Tree != null) bt_Tree.Stop();

        // Load the new tree
        bt_Tree = _bt_Tree;
        bt_Blackboard = bt_Tree.Blackboard;

        #if UNITY_EDITOR
        Debugger debugger = (Debugger)this.gameObject.AddComponent((typeof(Debugger)));
        debugger.BehaviorTree = bt_Tree;
        #endif

        bt_Tree.Start();
    }

    Root SelectBehaviourTree(int TreeSelection)
    {
        switch(TreeSelection)
        {
            case AGENT_IDLE:
                return Idle();

            case AGENT_DRINK:
                return SeekWater();

            case AGENT_EAT:
                return SeekFood();

            case AGENT_CHOP_WOOD:
                return GoToWoodAndChop();

            default:
                return Idle();
        }
    }

    /*********************************************
     * 
     * BASIC ACTIONS
     * 
     */

    void NavigateTo(Vector3 TargetPos)
    {
        AgentPathfinding.RequestPath(TargetPos);
    }

    void StartWander()
    {
        AgentSteering.SetWander = true;
    }

    void StopWander()
    {
        AgentSteering.SetWander = false;
    }

    bool JustReturnFalse()
    {
        return false;
    }

    /*********************************************
     * 
     * BEHAVIOUR TREES
     * 
     */

    Root Idle()
    {
        return new Root(new Action(() => StartWander()));
    }

    // If agent knows the location of a water source, go to it and drink
    // Otherwise wander until the agent finds it
    Root SeekWater()
    {
        return new Root(new Selector(
                        new Condition(() => WaterLocations.Count > 0, Stops.IMMEDIATE_RESTART,
                        GoToWaterAndDrink()),
                        WanderForWater()));
    }

    Node WanderForWater()
    {
        return new Sequence(
                    new Action(() => CurrentAgentCommunicate.UIBark("Water", "Where")),
                    new Action(() => StartWander()));
    }

    Node GoToWaterAndDrink()
    {
        return new Sequence(
                    new Action(() => StopWander()),
                    new Action(() => Debug.Log("Navigating to water")),
                    new Action(() => NavigateTo(GetClosest(WaterLocations))),
                    new Action(() => Debug.Log(GetClosest(WaterLocations))),
                    new Action(() => CurrentAgentCommunicate.UIBark("Water")),
                    new Wait(1f),
                    new WaitForCondition(() => !IsFollowingPath,
                        new Sequence(
                            new Action(() => CurrentAgentCommunicate.UIBarkStop()),
                            new Wait(0.5f),
                            new Action(() => CurrentAgentStatus.Drink(4.5f)))),
                    new WaitUntilStopped());
    }

    // If agent knows the location of a food source, go to it and eat
    // Otherwise wander until the agent finds it
    Root SeekFood()
    {
        return new Root(new Selector(
                        new Condition(() => FoodLocations.Count > 0, Stops.IMMEDIATE_RESTART,
                        GoToFoodAndEat()),
                        WanderForFood()));
    }

    Node WanderForFood()
    {
        return new Sequence(
                    new Action(() => CurrentAgentCommunicate.UIBark("Food", "Where")),
                    new Action(() => StartWander()));
    }

    Node GoToFoodAndEat()
    {
        return new Sequence(
                    new Action(() => StopWander()),
                    new Action(() => NavigateTo(GetClosest(FoodLocations))),
                    //new Action(() => Debug.Log(GetClosest(FoodLocations))),
                    new Action(() => CurrentAgentCommunicate.UIBark("Food")),
                    new Wait(1f),
                    new WaitForCondition(() => !IsFollowingPath,
                        new Sequence(
                            new Action(() => CurrentAgentCommunicate.UIBarkStop()),
                            new Wait(0.5f),
                            new Action(() => CurrentAgentStatus.Eat(4.5f)))),
                    new WaitUntilStopped());
    }

    Root GoToWoodAndChop()
    {
        return new Root(new Sequence(
                        new Action(() => StopWander()),
                        new Action(() => NavigateTo(GetClosest(WoodLocations))),
                        new Action(() => Debug.Log(GetClosest(WoodLocations))),
                        new Action(() => CurrentAgentCommunicate.UIBark("Wood")),
                        new Wait(1f),
                        new WaitForCondition(() => !IsFollowingPath,
                            new Sequence(
                                new Action(() => CurrentAgentCommunicate.UIBarkStop()),
                                new Wait(0.5f),
                                new Action(() => CurrentAgentStatus.Eat(4.5f)))),
                        new WaitUntilStopped()));
    }
}
