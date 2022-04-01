using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NPBehave;

public class AgentBT : MonoBehaviour
{
    // Reference to agent's path request and following script
    public GetPathAndFollow AgentPathfinding;

    // References to interactable objects in the world
    public Transform Water;
    public Transform Food;
    public Transform Wood;

    public Transform EasyTarget;

    Root bt_Tree;
    Blackboard bt_Blackboard;

    // List of behaviour trees
    const int AGENT_IDLE = 0;
    const int AGENT_SEEK_WATER = 1;
    const int AGENT_SEEK_FOOD = 2;
    const int AGENT_CHOP_WOOD = 3;

    int CurrentAction;
    List<int> UtilityScores;

    // Start is called before the first frame update
    void Start()
    {
        AgentPathfinding = GetComponent<GetPathAndFollow>();

        // Get interaction locations for interactable objects
        Water = Water.Find("InteractionTarget");
        Food = Food.transform.Find("InteractionTarget");
        Wood = Wood.transform.Find("InteractionTarget");

        // Set initial action
        CurrentAction = AGENT_IDLE;
        SwitchTree(SelectBehaviourTree(CurrentAction));

        // Set utility scores to zero
        UtilityScores = new List<int>();
        UtilityScores.Add(0); // AGENT_IDLE
        UtilityScores.Add(0); // AGENT_SEEK_WATER
        UtilityScores.Add(0); // AGENT_SEEK_FOOD
        UtilityScores.Add(0); // AGENT_CHOP_WOOD
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScores();
        int maxValue = UtilityScores.Max(t => t);
        int maxIndex = UtilityScores.IndexOf(maxValue);

        if (CurrentAction != maxIndex)
        {
            CurrentAction = maxIndex;
            SwitchTree(SelectBehaviourTree(CurrentAction));
        }
    }

    void UpdateScores()
    {
        UtilityScores[AGENT_IDLE] = 10;
        UtilityScores[AGENT_SEEK_WATER] = 15;
        UtilityScores[AGENT_SEEK_FOOD] = 5;
        UtilityScores[AGENT_CHOP_WOOD] = 5;
    }

    void SwitchTree(Root _bt_Tree)
    {
        // Stop the currently running tree (if there is one)
        if (bt_Tree != null) bt_Tree.Stop();

        // Load the new tree
        bt_Tree = _bt_Tree;
        bt_Blackboard = bt_Tree.Blackboard;
        
        bt_Tree.Start();
    }

    Root SelectBehaviourTree(int TreeSelection)
    {
        switch(TreeSelection)
        {
            case AGENT_IDLE:
                return Idle();

            case AGENT_SEEK_WATER:
                return SeekWater();

            case AGENT_SEEK_FOOD:
                return SeekFood();

            case AGENT_CHOP_WOOD:
                return ChopWood();

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

    void Wander()
    {
        // Complete once steering is finished
    }

    /*********************************************
     * 
     * BEHAVIOUR TREES
     * 
     */

    Root Idle()
    {
        return new Root(new Action(() => Wander()));
    }

    Root SeekWater()
    {
        /*return new Root(new Sequence(
                        new Action(() => NavigateTo(Water.position)),
                        new Wait(60.0f)));*/

        return new Root(new Sequence(
                        new Action(() => NavigateTo(Water.position)),
                        new Wait(60.0f)));
    }

    Root SeekFood()
    {
        return new Root(new Action(() => NavigateTo(Food.position)));
    }

    Root ChopWood()
    {
        return new Root(new Action(() => NavigateTo(Wood.position)));
    }
}
