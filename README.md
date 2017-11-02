# AgentBasedLevelGenerator
A Unity implementation of a procedural level generator based on one or more agents moving in a grid. If there are two agents creating the level, a simple Depth-First Search is performed to check whether the level is connected and a simple corridor is generated between the agents if necessary. The generator also automatically builds a navmesh of the generated map.

# Screenshots
![Screenshot 1](/Screenshots/snapshot1.PNG?raw=true "Screenshot 1")

![Screenshot 2](/Screenshots/snapshot2.PNG?raw=true "Screenshot 2")

![Screenshot 3](/Screenshots/snapshot3.PNG?raw=true "Screenshot 3")

# Minimap
The code creates a render of the level as a minimap
![It has a minimap!](/Screenshots/snapshot4_minimap.PNG?raw=true "Minimap")

# Navmesh
The navmesh is automatically generated based on: https://github.com/Unity-Technologies/NavMeshComponents
![NavMesh](/Screenshots/navmesh.PNG?raw=true "Navmesh Example")
