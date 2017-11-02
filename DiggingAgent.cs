using System.Collections.Generic;
using System.Collections;

public class DiggingAgent
{
    public IntVector2 pos;
    public GridDirection direction;
    public float turnProb;
    public float roomProb;
    public LDCell CurrentCell { get { return level.GetCell(pos); } }
    public int stepsDone;

    protected float base_changeProb;
    protected float base_roomprob;
    protected LevelDigger level;

    //Material indicatorColor;
    
    public DiggingAgent(LevelDigger _level, IntVector2 position, GridDirection init_direction, float init_changeProb, float init_roomProb)
    {
        level = _level;
        pos = position;
        direction = init_direction;
        base_changeProb = turnProb = init_changeProb;
        base_roomprob = roomProb = init_roomProb;
        stepsDone = 0;
    }

    public void OpenCurrentCell()
    {
        if(!CurrentCell.IsOpen)
            CurrentCell.SetOpen(true);
    }

    public void Highlight()
    {
        CurrentCell.Highlight();
    }

    public void UnHighlight()
    {
        CurrentCell.UnHighlight();
    }

}
