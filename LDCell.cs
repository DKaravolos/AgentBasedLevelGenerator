using UnityEngine;

public class LDCell : MonoBehaviour
{
    public IntVector2 coordinates;
    public bool IsOpen { get { return !closedForm.activeInHierarchy; } }
    public bool CanChangeColor { get; set; }
    [HideInInspector]
    public LDRoom room;
    [SerializeField]
    private GameObject closedForm;
    [SerializeField]
    private GameObject openForm;
    [SerializeField]
    private GameObject indicator;

    public LDCell()
    {
        ////we should be able to find these automatically, but I get a weird bug.
        //closedForm = transform.GetChild(0).gameObject;
        //openForm = transform.GetChild(1).gameObject;
        //indicator = transform.GetChild(2).gameObject;
        room = null;
        CanChangeColor = true;
    }

    public void SetOpen(bool open)
    {
        closedForm.SetActive(!open);
        openForm.SetActive(open);
    }

    public void AddToRoom(LDRoom _room, bool fixColor)
    {
        room = _room;
        room.Add(this);
        openForm.SetActive(true);
        closedForm.SetActive(false);
        if(CanChangeColor)
        {
            openForm.GetComponent<Renderer>().material = room.settings.floorMaterial;  // We only have a floor
            CanChangeColor = fixColor;
        }
    }

    public void Highlight()
    {
        indicator.SetActive(true);
    }
    public void UnHighlight()
    {
        indicator.SetActive(false);
    }

    //We might want to not show a cell at all if it is not a usable part of the map
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
