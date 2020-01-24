# AgentBasedLevelGenerator
A Unity implementation of a procedural level generator based on one or more agents moving in a grid. If there are one or more agents creating the level, a simple Depth-First Search is performed to check whether the level is connected and a simple corridor is generated between the agents if necessary. The size of the map, the sizes and probabilities of the rooms and whether or not rooms can overlap are all changeable parameters of the algorithm (part of the MultiAgentDigger prefab). The generator also automatically builds a navmesh of the generated map. New maps can be generated (with different parameters if you wish) while the scene is running.

# Tutorial
For the Institute of Digital Games at the University of Malta, I created a tutorial in which students need to fill in the most important parts of the code. The tutorial can be found here: [link](https://github.com/DKaravolos/AgentBasedLevelGenerator/blob/master/AgentBasedLevelGenerator_Tutorial.zip). 
The solution can be found here: [link](https://github.com/DKaravolos/AgentBasedLevelGenerator/blob/master/AgentBasedLevelGenerator_Tutorial_Solution.zip) and is probably better documented than the main code in this repo. Sorry for that. 

# Hierarchical Generator
A more elaborate generator, used for creating the data set of my PhD thesis can be found here: [link](https://drive.google.com/open?id=1B4m5XwY5fk6RjYIgG7YFPonKj0v3nc9c). This generator assures that there are two paths between the two bases and has two walkable floor levels. For more information, check out [my personal website](https://danielkaravolos.nl/publications/dissertation/)

# ASCI Map
The solution to the tutorial contains code for exporting the map to a csv-file of ASCI characters (basically 0,1 and 2 for the different floor levels).

# Screenshots
![Gif 1v10](https://github.com/DKaravolos/AgentBasedLevelGenerator/blob/master/1vs10.gif)

![Gif 1vs10](https://github.com/DKaravolos/AgentBasedLevelGenerator/blob/master/1vs10_2.gif)

![Screenshot 1](/Screenshots/snapshot1.PNG?raw=true "Screenshot 1")

![Screenshot 2](/Screenshots/snapshot2.PNG?raw=true "Screenshot 2")

![Screenshot 3](/Screenshots/snapshot3.PNG?raw=true "Screenshot 3")

# Minimap
The code creates a render of the level as a minimap
![It has a minimap!](/Screenshots/snapshot4_minimap.PNG?raw=true "Minimap")

# Navmesh
The navmesh is automatically generated based on: https://github.com/Unity-Technologies/NavMeshComponents
![NavMesh](/Screenshots/navmesh.PNG?raw=true "Navmesh Example")
