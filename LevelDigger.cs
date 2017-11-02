using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDigger : MonoBehaviour
{
    //Public
    [Header("Prefab stuff")]
    public LDCell cellPrefab;
    public GameObject undergroundPrefab;
    public LDRoomSettings[] roomSettings;
    public bool onlyFirstColor = true;

    [Header("Display Properties")]
    public float generationStepDelay = 0.01f;

    [Header("Level Size Properties")]
    public Vector3 cellScale = new Vector3(1, 1, 1);
    public IntVector2 size;

    [Header("Digging Agent Properties")]
    public int maxNrSteps = 50;
    
    [Range(0f, 1f)]
    public float changeDirectionProb;
    [Range(0f, 1f)]
    public float makeRoomProb;
    public bool dynamicProbabilities = true;
    public float changeDirDelta = 0.05f;
    public float makeRoomDelta = 0.05f;

    public int minRoomSize = 3;
    public int maxRoomSize = 9;
    
    public bool startWithRoom = true;
    public bool endWithRoom = false;
    public bool fixStartPosition;
    public IntVector2 startPosition = new IntVector2(0, 0);
    

    //Privates
    protected LDCell[,] cells;
    protected GameObject underground;
    protected List<LDRoom> rooms = new List<LDRoom>();

    public IntVector2 RandomCoordinates
    {
        get
        {
            return new IntVector2(Random.Range(0, size.x), Random.Range(0, size.z));
        }
    }

    public int[] RandomRoomSize
    {
        get
        {
            int x = Random.Range(minRoomSize, maxRoomSize);
            int z = Random.Range(minRoomSize, maxRoomSize);
            int[] array = { x, z };
            return array;
        }
    }

    public bool ContainsCoordinates(IntVector2 coordinate)
    {
        return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
    }

    public LDCell GetCell(IntVector2 coordinates)
    {
        return cells[coordinates.x, coordinates.z];
    }

    virtual protected void Init()
    {
        cells = new LDCell[size.x, size.z];
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                IntVector2 coordinates = new IntVector2(i, j);
                CreateCell(coordinates, cellScale, false);
            }
        }
        //This creates an object below our level. Mainly for debugging size/position problems.
        underground = Instantiate(undergroundPrefab);
        underground.transform.parent = transform;
        underground.transform.localScale = new Vector3(size.x * cellScale.x, 1, size.z * cellScale.z);
    }
    
    //This is the mainloop called by the GameManager
    virtual public IEnumerator Generate()
    {
        WaitForSeconds delay = new WaitForSeconds(generationStepDelay);
        Init();

        //Note that the single-agent level digger does not actually have an agent.
        //It has separate variables for position, direction, etc.
        IntVector2 agentPosition;
        GridDirection agentDirection = GridDirections.RandomValue;
        if (fixStartPosition)
            agentPosition = startPosition;
        else
            agentPosition = RandomCoordinates;
        LDCell currCell = GetCell(agentPosition);

        int steps = 0;
        //Poor man's "Create First Spawn Room"
        if (startWithRoom)
        {
            rooms.Add(CreateRoom(agentPosition, RandomRoomSize));
            currCell.Highlight();
            steps++;
            yield return delay;
        }

        //Main Loop
        float turnProb = changeDirectionProb;
        float roomProb = makeRoomProb;
        while (steps < maxNrSteps)
        {
            Debug.Log("Step " + steps);
            currCell.Highlight();
            yield return delay;
            currCell.UnHighlight();
            DoNextGenerationStep(ref agentPosition, ref agentDirection, ref currCell, ref turnProb, ref roomProb);
            steps++;
        }

        //Poor man's "Create Second Spawn Room"
        if (endWithRoom)
        {
            rooms.Add(CreateRoom(agentPosition, RandomRoomSize));
        }

        //Make everything static for navmesh generation
        foreach (LDCell cell in cells)
        {
            cell.gameObject.isStatic = true;
        }

        yield return delay;
    }

    virtual protected void DoNextGenerationStep(ref IntVector2 currentPosition, ref GridDirection currentDirection, ref LDCell currentCell,
        ref float turnProb, ref float roomProb)
    {
        //First we move the agent.
        //We change direction by chance or if the next position in the current direction is not in the level.
        if (Random.value < changeDirectionProb || !ContainsCoordinates(currentPosition + currentDirection.ToIntVector2()))
        {
            //Randomly move the agent to a new valid position in the level.
            GridDirection newDir = ChangeDirection(currentPosition, currentDirection);
            currentDirection = newDir;
            turnProb = changeDirectionProb;
        }
        else
            if(dynamicProbabilities)
                turnProb += changeDirDelta;
        //Now we now the next position! 
        currentPosition += currentDirection.ToIntVector2();

        //Make a room?
        if(Random.value < roomProb)
        {
            rooms.Add(CreateRoom(currentPosition, RandomRoomSize));
            roomProb = makeRoomProb;
        }
        else 
        {
            //else just open current cell
            currentCell = GetCell(currentPosition);
            if (!currentCell.IsOpen)
                currentCell.SetOpen(true);
            
            if (dynamicProbabilities)
                roomProb += makeRoomDelta;
        }

    }

    protected GridDirection ChangeDirection(IntVector2 currentPosition, GridDirection currentDirection)
    {
        IntVector2 newPos;
        GridDirection newDir;
        do
        {
            newDir = GridDirections.RandomValue;
            newPos = currentPosition + newDir.ToIntVector2();
        } while (newDir == currentDirection || !ContainsCoordinates(newPos));
        return newDir;
    }

    //The CreateCell is only used at the start to setup the complete grid.
    //After that, you can just access the cell and open or close it
    //Special cell content should probably be added when creating "rooms".
    protected LDCell CreateCell(IntVector2 coordinates, Vector3 cellScale, bool open)
    {
        LDCell newCell = Instantiate(cellPrefab) as LDCell;
        newCell.SetOpen(open);
        newCell.transform.localScale = cellScale;
        cells[coordinates.x, coordinates.z] = newCell;
        newCell.name = "LD Cell " + coordinates.x + ", " + coordinates.z;
        newCell.coordinates = coordinates;
        newCell.transform.parent = transform;
        newCell.transform.localPosition = new Vector3(cellScale.x * (coordinates.x - size.x * 0.5f + 0.5f), cellScale.y * 0.5f, cellScale.z * (coordinates.z - size.z * 0.5f + 0.5f));
        return newCell;
    }

    //This function creates rooms. Currently it is only used to give tiles a color.
    //fixColor indicates whether the color of the cell can change after it is assigned to this room.
    protected LDRoom CreateRoom(IntVector2 currentPos, int[] roomSize, bool fixColor=false)
    {
        LDRoomSettings setting;
        if (onlyFirstColor)
            setting = roomSettings[0];
        else
            setting = roomSettings[Random.Range(0, roomSettings.Length)];
        return CreateRoom(currentPos, roomSize, setting, fixColor);
    }

    //This function creates rooms with a specified setting. Currently it is only used to give tiles a color.
    protected LDRoom CreateRoom(IntVector2 currentPos, int[] roomSize, LDRoomSettings setting, bool fixColor)
    {
        
        LDRoom newRoom = ScriptableObject.CreateInstance<LDRoom>();
        //Check if there already is a room here. In that case, we want to add the new cells to that room
        //If there is a room, it is stored in newRoom
        if (!OverlapsRoom(currentPos, roomSize, ref newRoom))
        {
            Debug.Log(string.Format("Creating Room of [{0}, {1}], color: {2}", roomSize[0], roomSize[1], setting.floorMaterial.name));
            newRoom.settings = setting;
        }
        else
            Debug.Log("Expanding Room.");
        
        //Add cells to the room, whether it is new or not
        for (int x = currentPos.x - roomSize[0]/ 2; x < currentPos.x + roomSize[0] / 2; x++)
            for (int z = currentPos.z - roomSize[1] / 2; z < currentPos.z + roomSize[1] / 2; z++)
            {
                if (ContainsCoordinates(new IntVector2(x,z)))
                    cells[x, z].AddToRoom(newRoom, fixColor);
            }
        return newRoom;
    }

    protected bool OverlapsRoom(IntVector2 currentPos, int[] roomSize, ref LDRoom room)
    {
        for (int x = currentPos.x - roomSize[0] / 2; x < currentPos.x + roomSize[0] / 2; x++)
            for (int z = currentPos.z - roomSize[1] / 2; z < currentPos.z + roomSize[1] / 2; z++)
            {
                if (ContainsCoordinates(new IntVector2(x, z)))
                    if (cells[x, z].room != null)
                    {
                        room = cells[x, z].room;
                        return true;
                    }
            }
        return false;
    }

    //Use a simple Depth-First Search to check if there is a path between two points
    protected bool HasPathBetween(IntVector2 firstPos, IntVector2 secondPos, List<IntVector2> visited)
    {
        if (!ContainsCoordinates(firstPos))
            return false;

        if (!GetCell(firstPos).IsOpen || visited.Contains(firstPos))
            return false;

        visited.Add(firstPos);
        if (firstPos.x == secondPos.x && firstPos.z == secondPos.z)
        {
            return true;
        }
        if (HasPathBetween(firstPos + GridDirection.North.ToIntVector2(), secondPos, visited)) {return true; }
        if (HasPathBetween(firstPos + GridDirection.East.ToIntVector2(), secondPos, visited)) { return true; }
        if (HasPathBetween(firstPos + GridDirection.South.ToIntVector2(), secondPos, visited)) { return true; }
        if (HasPathBetween(firstPos + GridDirection.West.ToIntVector2(), secondPos, visited)) { return true; }
        return false;
    }

    //Opens all cells between two points, based on a simple greedy algorithm: first move in X direction, then move in Z direction
    protected void ConnectPositions(IntVector2 pos1, IntVector2 pos2)
    {
        int xDiff = pos2.x - pos1.x;
        if(xDiff > 0)
        {
            for (int i = 0; i < xDiff; i++)
            {
                pos1 += GridDirection.East.ToIntVector2();
                GetCell(pos1).SetOpen(true);
            }
        }
        if (xDiff < 0)
        {
            for (int i = 0; i > xDiff; i--)
            {
                pos1 += GridDirection.West.ToIntVector2();
                GetCell(pos1).SetOpen(true);
            }
        }

        int zDiff = pos2.z - pos1.z;
        if (zDiff > 0)
        {
            for (int i = 0; i < zDiff; i++)
            {
                pos1 += GridDirection.North.ToIntVector2();
                GetCell(pos1).SetOpen(true);
            }
        }
        if (zDiff < 0)
        {
            for (int i = 0; i > zDiff; i--)
            {
                pos1 += GridDirection.South.ToIntVector2();
                GetCell(pos1).SetOpen(true);
            }
        }

    }
}
