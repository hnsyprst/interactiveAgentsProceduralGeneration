# Introduction
This project implements an interactive technical demonstration of several game AI techniques in Unity. Two types of agents are created: a Person agent, which uses a utility-based behaviour tree system and A* pathfinding to seek and consume food and water; and a Duck agent, which uses steering behaviours to randomly roam the environment. Two procedural generation systems are built: a binary space partitioning system, which divides the map into themed chunks; and a wave function collapse system, which uses tiles to fill each chunk with relevant resources and decoration.
## Player Interaction
To interact with the game, the player can click anywhere on the map that does not contain an obstacle. The Person Agents will use the pathfinding system to navigate to the clicked location; this can be used to help the Person Agents find food and water sources.

# Agents
## Person Agent

### Navigation
Person Agents use the A* algorithm for pathfinding, using an implementation informed by Sebastian Lague's series of tutorials (2014). After the map has been created by the Generator, [Assets/Agent/Navigation/Pathfinding/Grid.cs](https://github.com/hnsyprst/interactiveAgentsProceduralGeneration/blob/main/Assets/Agent/Navigation/Pathfinding/Grid.cs) divides the map into a regular grid of [Assets/Agent/Navigation/Pathfinding/PFNode.cs](https://github.com/hnsyprst/interactiveAgentsProceduralGeneration/blob/main/Assets/Agent/Navigation/Pathfinding/PFNode.cs). Each PFNode contains information about their position in the world, including their X and Y coordinates and whether or not they contain obstacles. The grid is drawn using gizmos in the scene view when the game is run. White tiles represent walkable spaces and red tiles represent obstacles.

Once the grid has been generated, [Assets/Agent/Navigation/Pathfinding/Pathfinding.cs](https://github.com/hnsyprst/interactiveAgentsProceduralGeneration/blob/main/Assets/Agent/Navigation/Pathfinding/Pathfinding.cs) can be used to find paths across the map via the A* algorithm. Starting and target positions can be passed directly to Pathfinding, which uses Grid to locate the closest PFNode to the given position.

For performance, Pathfinding is accessed via [Assets/Agent/Navigation/Pathfinding/PathRequestManager.cs](https://github.com/hnsyprst/interactiveAgentsProceduralGeneration/blob/main/Assets/Agent/Navigation/Pathfinding/PathRequestManager.cs). This script creates a queue of pathfinding requests, passing the next in the queue to Pathfinding's FindPath() coroutine when the previous request has been completed.

### Behaviour
Person Agents each use a utility-based behaviour tree system for decision making. The behaviour trees are created using NPBehave. [Assets/Agent/Navigation/Behaviour/AgentBT.cs](https://github.com/hnsyprst/interactiveAgentsProceduralGeneration/blob/main/Assets/Agent/Navigation/Behaviour/AgentBT.cs) implements five behaviour trees:

 1. AGENT_IDLE
	 - Currently, this behaviour tree simply returns false, meaning that the agent will just stand still. In future, this tree could be updated to implement more interesting idling behaviour.
 2. AGENT_DRINK
	 - Activates the SeekWater() Root node, which causes the agent to choose random locations to navigate to (calling PathRequestManager to generate paths) until or unless a water source has been found. Once a water source has been found, the agent will navigate to the source and call it's Drink() action once it has arrived, lowering the agent's Thirst (and thus the utility of AGENT_DRINK).
 3. AGENT_EAT
	 - Activates the SeekFood() Root node, which causes the agent to choose random locations to navigate to (calling PathRequestManager to generate paths) until or unless a food source has been found. Once a food source has been found, the agent will navigate to the source and call it's Eat() action once it has arrived, lowering the agent's Hunger (and thus the utility of AGENT_EAT).
 4. AGENT_CHOP_WOOD
	 - Currently, this behaviour will cause the agent to choose random locations to navigate to (calling PathRequestManager to generate paths) until or unless a wood source has been found. Rather than chopping wood, however, the agent will call its Eat() action once it has arrived. In future, a behaviour should be implemented to allow the agent to gather wood from trees and return it to the building in the centre of the map.
 5. PLAYER_CLICKED
	 - The player can interact with the Person Agents by clicking anywhere in the environment. This behaviour tree causes the agents to navigate to the clicked location, and reduces the utility of PLAYER_CLICKED once they have arrived, allowing the agents to return to their normal behaviour.

Behaviour trees are switched between using a utility system. [Assets/Agent/Navigation/Behaviour/AgentStatus.cs](https://github.com/hnsyprst/interactiveAgentsProceduralGeneration/blob/main/Assets/Agent/Navigation/Behaviour/AgentStatus.cs) creates Hunger and Thirst variables, constantly increasing  the value of each over time. These variables are used by AgentBT to update the utility scores of the AGENT_DRINK and AGENT_EAT behaviour trees each frame---the behaviour tree with the highest value is then switched to. When the player interacts with the environment, a UnityEvent callback is invoked, which sets the utility of the PLAYER_CLICKED behaviour tree to 100, meaning that agents will always switch to this tree when the player clicks. Currently, the utility scores for AGENT_IDLE and AGENT_CHOP_WOOD are both set to 0, meaning these trees will never be switched to. In the future, agents hunger and thirst should not increase for a small amount of time after they have eaten or drunk, so that idle or wood chopping behaviours can be accessed.

Agents do not know the locations of food and water at the start of the game---they must wander randomly around the environment until they find a source. This is achieved using a perception system implemented in [Assets/Agent/Navigation/Behaviour/AgentPerception.cs](https://github.com/hnsyprst/interactiveAgentsProceduralGeneration/blob/main/Assets/Agent/Navigation/Behaviour/AgentPerception.cs). While agents are moving around the environment, Physics.Raycast whiskers in each of the eight body relative directions along the X and Z axes are cast. When a resource is hit by one of the whiskers, it is added to the agent's HashSet for that resource type (HashSet are used so that each individual resource is not added to the collection more than once). The agents can query each HashSet from AgentBT as needed to check if they have discovered a resource. The whiskers are drawn using gizmos in the scene view. The whiskers are red when they have not detected a resource, and green when they have.

According to Mark Brown, "good AI tells you what it's thinking" (Game Maker's Toolkit, 2017). For this reason, Person Agents use the UI to communicate with the player via [Assets/Agent/Navigation/Behaviour/AgentCommunicate.cs](https://github.com/hnsyprst/interactiveAgentsProceduralGeneration/blob/main/Assets/Agent/Navigation/Behaviour/AgentCommunicate.cs). The script accesses a library of open source emojis from [Twemoji](https://twemoji.twitter.com/) to inform the player when the agent is searching for food or water sources, navigating to the player's clicked location or navigating to a known food or water source.

## Duck Agent
Duck agents use [Assets/Agent/Navigation/Steering/Wander.cs](https://github.com/hnsyprst/interactiveAgentsProceduralGeneration/blob/main/Assets/Agent/Navigation/Steering/Wander.cs) to randomly move around the environment. Wander implements the wander steering behaviour (Reynolds, 1999), generating small displacements to the current direction of travel to simulate casual movement. These displacement forces are passed to [Assets/Agent/Navigation/Steering/Steer.cs](https://github.com/hnsyprst/interactiveAgentsProceduralGeneration/blob/main/Assets/Agent/Navigation/Steering/Steer.cs), which clamps their magnitude and applies them to the Duck agent.

# Generator
Inspired by the system used in Caves of Qud (Freehold Games, LLC, 2015), this game's map generator first divides the available space using a binary space partitioning system, selects a theme for each divided space and uses a wave function collapse system to fill each space with the tiles applicable to its theme.

## Binary Space Partitioning

To generate the map, the available space is first partitioned using a binary space partitioning system informed by Sunny Valley Studio's series of tutorials (Sunny Valley Studio, 2019). [Assets/Generator/SpacePartitioner.cs](https://github.com/hnsyprst/interactiveAgentsProceduralGeneration/blob/main/Assets/Generator/SpacePartitioner.cs) calls [Assets/Generator/SpaceGenerator.cs](https://github.com/hnsyprst/interactiveAgentsProceduralGeneration/blob/main/Assets/Generator/SpaceGenerator.cs)'s CalculateRooms() method, which creates a new [Assets/Generator/BinarySpacePartitioner.cs](https://github.com/hnsyprst/interactiveAgentsProceduralGeneration/blob/main/Assets/Generator/BinarySpacePartitioner.cs) object. BinarySpacePartitioner recursively splits the given space into sub-spaces---instances of [Assets/Generator/RoomSPNode.cs](https://github.com/hnsyprst/interactiveAgentsProceduralGeneration/blob/main/Assets/Generator/RoomSPNode.cs), a class containing each sub-space's world coordinates and position in the division graph---by generating either a horizontal or vertical dividing line ([Assets/Generator/SPLine.cs](https://github.com/hnsyprst/interactiveAgentsProceduralGeneration/blob/main/Assets/Generator/SPLine.cs)) across the space at intervals of 5 units. This interval is chosen as it is the size of one tile that will later be used by the wave function collapse algorithm; this ensures the map is filled with no overlaps or gaps.

Once BinarySpacePartitioner has recursively divided the given space for the number of iterations specified, the list of RoomSPNodes is returned to SpaceGenerator. Since each space may be divided multiple times, this list will be a graph containing the original room, the first two sub-spaces, each of those sub-spaces' sub-spaces, and so on. In order to process only the undivided spaces, the graph is traversed to find its leaves. These leaves are returned to SpacePartitioner.

The divided space is drawn using gizmos in the scene view when the game is run. Each leaf is drawn in a random colour.

## Wave Function Collapse

SpacePartitioner is originally called by [Assets/Generator/MapGenerator.cs](https://github.com/hnsyprst/interactiveAgentsProceduralGeneration/blob/main/Assets/Generator/MapGenerator.cs). Each leaf returned to SpacePartitioner by SpaceGenerator is passed to a new wave function collapse system, which randomly chooses a theme for the space and fills it with that theme's tiles. The wave function collapse system is created using Tessera.

The tiles for each theme are denoted in lists. Each tile is given a weight, determining the probability that it will be used by the wave function collapse system. This allows for different spaces to be filled with different resources---bushes areas will be filled with food resources, water areas with water resources, etc. Each theme is only generated once until all themes have been used, in order to avoid the entire map being filled with the tiles from one theme. 

Constraints are applied to the wave function collapse systems which force the placement of a building in the centre of the map. In future, this building will act as a resource drop-off point for Person Agents. A volume is placed around the building using a Box Collider which prevents the wave function collapse systems from placing any tiles other than Ground and Water. This prevents the player's view of the building from being obscured by berries or trees, and helps to prevent the Person Agents from becoming trapped.

# Third Party Assets

 - [NPBehave](https://github.com/meniku/NPBehave) is used to implement behaviour trees
 - [Tessera](https://assetstore.unity.com/packages/tools/level-design/tessera-procedural-tile-based-generator-155425) is used to implement wave function collapse
 - [TextMeshPro](https://docs.unity3d.com/Manual/com.unity.textmeshpro.html) is used to implement Person Agent UI barks
 - [Twemoji](https://twemoji.twitter.com/) emojis are used to communicate with the player

# References
Lague, S. (2014) _A* Pathfinding Tutorial_. 16 December. Available at: https://www.youtube.com/watch?v=-L-WgKMFuhE&list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW [Accessed: 25/04/2022].

Reynolds, C. W. (1999) 'Steering Behaviors For Autonomous Characters', _Proceedings of Game Developers Conference 1999_, San Jose, California, 19 March. Pages 763-782. Available at: https://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.86.636&rep=rep1&type=pdf [Accessed: 25/04/2022].

Game Maker's Toolkit (2017) _What Makes Good AI?_. 31 May. Available at: https://www.youtube.com/watch?v=9bbhJi0NBkk [Accessed: 25/04/2022].

Sunny Valley Studio (2019) _Unity 3d Procedural Dungeon Generator_. 18 November. Available at: https://www.youtube.com/watch?v=VnqN0v95jtU&list=PLcRSafycjWFfEPbSSjGMNY-goOZTuBPMW [Accessed: 25/04/2022].

Freehold Games, LLC (2015) _Caves of Qud_ [Video game]. Freehold Games, LLC.
