using UnityEngine;
using System.Collections.Generic;

public class LDRoom : ScriptableObject
{

    public int settingsIndex;

    public LDRoomSettings settings;

    private List<LDCell> cells = new List<LDCell>();
    public List<LDCell> ReadOnlyCells
    {
        get { return cells; }
    }

    public void Add(LDCell cell)
    {
        cell.room = this;
        cells.Add(cell);
    }

    //We might want to not show a room at all if it is not a usable part of the map or for other reasons...
    public void Hide()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].Hide();
        }
    }

    public void Show()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].Show();
        }
    }
}