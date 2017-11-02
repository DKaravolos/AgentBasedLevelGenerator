using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class MultiAgentDigger : LevelDigger {
    public List<IntVector2> agentPositions;  //You specify both the nr of agents and their start positions in the editor.
    public List<IntVector2> startRoomSizes;
    public List<GridDirection> startDirections;
    public bool randomStartRooms = false;
    public bool colorStartRooms = true;

    protected List<DiggingAgent> agents;       //This will keep track of the agents.

    protected override void Init()
    {
        base.Init();
        agents = new List<DiggingAgent>(agentPositions.Count);
        for (int agent = 0; agent < agentPositions.Count; agent++)
        {
            GridDirection dir;
            if (startDirections != null)
                dir = startDirections[agent];  //We are assuming that if startDirections exists, its size is equal to nr of agents.
            else
                dir = GridDirections.RandomValue;
            DiggingAgent da = new DiggingAgent(this, agentPositions[agent], dir, changeDirectionProb, makeRoomProb);
            agents.Add(da);
        }
    }

    public override IEnumerator Generate()
    {
        WaitForSeconds delay = new WaitForSeconds(generationStepDelay);
        Init();

        //Create spawn rooms
        if (startWithRoom)
        {
            for (int agent = 0; agent < agents.Count; agent++)
            {
                int[] roomSize;
                if (randomStartRooms)
                    roomSize = RandomRoomSize;
                else
                    roomSize = new int[]{ startRoomSizes[agent].x, startRoomSizes[agent].z};

                rooms.Add(CreateRoom(agents[agent].pos, roomSize, roomSettings[agent+1], false));
                agents[agent].CurrentCell.Highlight();
                agents[agent].stepsDone++;
            }
            yield return delay;
        }

        //This is the main loop that digs the level.
        int steps = 0;
        while (steps < maxNrSteps)
        {
            Debug.Log("Step " + steps);
            //Show where the agents are
            foreach (var agent in agents)
                agent.Highlight();
            //pauze this process for visualization in the editor
            yield return delay;
            //Stop showing where the agents are and have each agent perform one step
            foreach (var agent in agents)
            {
                agent.UnHighlight();
                DoNextGenerationStep(agent);
            }
            steps++;
        }

        //Stand in for create second spawn room // probably should not do this with MultiAgentDigging
        if (endWithRoom)
        {
            for (int agent = 0; agent < agents.Count; agent++)
            {
                rooms.Add(CreateRoom(agents[agent].pos, RandomRoomSize));
            }
        }

        //Check whether paths are connected and if not, make a path
        CheckConnectedness();

        //Make everything static for navmesh generation
        foreach (LDCell cell in cells)
        {
            cell.gameObject.isStatic = true;
        }

        yield return delay;
    }

    protected void DoNextGenerationStep(DiggingAgent agent)
    {
        //First we move the agent.
        //We change direction by chance or if the next position in the current direction is not in the level.
        if (Random.value < changeDirectionProb || !ContainsCoordinates(agent.pos + agent.direction.ToIntVector2()))
        {
            //Randomly move the agent to a new valid position in the level.
            GridDirection newDir = ChangeDirection(agent.pos, agent.direction);
            agent.direction = newDir;
            agent.turnProb = changeDirectionProb;
        }
        else
            if (dynamicProbabilities)
                agent.turnProb += changeDirDelta;
        //Now we now the next position! 
        agent.pos += agent.direction.ToIntVector2();

        //Make a room?
        if (Random.value < agent.roomProb)
        {
            rooms.Add(CreateRoom(agent.pos, RandomRoomSize));
            agent.roomProb = makeRoomProb;
        }
        else
        {
            //else just open current cell
            agent.OpenCurrentCell();
            if (dynamicProbabilities)
                agent.roomProb += makeRoomDelta;
        }

    }

    protected void CheckConnectedness()
    {
        if(agents.Count == 1)
            return;
        if(agents.Count > 2)
        {
            Debug.LogWarning("Connectedness checking is not implemented for more than two agents");
            return;
        }
        List<IntVector2> visited = new List<IntVector2>();
        if(HasPathBetween(agents[0].pos, agents[1].pos, visited))
        {
            Debug.Log("The map is connected.");
        }
        else
        {
            Debug.Log("Connecting the map...");
            ConnectPositions(agents[0].pos, agents[1].pos);
            Debug.Log("Connecting map is done!");
        }
    }
}
