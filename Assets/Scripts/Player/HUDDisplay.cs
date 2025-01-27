using UnityEngine;

public class HUDDisplay : MonoBehaviour
{
    public GameObject dynamicInteractDisplay;
    public GameObject stationaryInteractDisplay;
    public GameObject carryDisplay;
    public GameObject stickyNoteDisplay;
    public GameObject markerDisplay;

    private HUDState state;

    public void SetState(HUDState stateToSet)
    {
        state = stateToSet;
    }

    private void Update()
    {
        state.Display();
    }

    public void DisableAllPrompts()
    {
        dynamicInteractDisplay.SetActive(false);
        stationaryInteractDisplay.SetActive(false);
        carryDisplay.SetActive(false);
        stickyNoteDisplay.SetActive(false);
        markerDisplay.SetActive(false);
    }

}

public class DynamicInteract : HUDState
{
    public DynamicInteract(HUDDisplay display) : base(display) { }
    public override void Display()
    {
        display.DisableAllPrompts();
        display.dynamicInteractDisplay.SetActive(true);
    }
}

public class StationaryInteract : HUDState
{
    public StationaryInteract(HUDDisplay display) : base(display) { }
    public override void Display()
    {
        display.DisableAllPrompts();
        display.stationaryInteractDisplay.SetActive(true);
    }
}

public class CarryDisplay : HUDState
{
    public CarryDisplay(HUDDisplay display) : base(display) { }
    public override void Display()
    {
        display.DisableAllPrompts();
        display.carryDisplay.SetActive(true);
    }
}

public class StickyNoteDisplay : HUDState
{
    public StickyNoteDisplay(HUDDisplay display) : base(display) { }
    public override void Display()
    {
        display.DisableAllPrompts();
        display.stickyNoteDisplay.SetActive(true);
    }
}

public class MarkerDisplay : HUDState
{
    public MarkerDisplay(HUDDisplay display) : base(display) { }
    public override void Display()
    {
        display.DisableAllPrompts();
        display.markerDisplay.SetActive(true);
    }
}

public class EmptyDisplay : HUDState
{
    public EmptyDisplay(HUDDisplay display) : base(display) { }
    public override void Display()
    {
        display.DisableAllPrompts();
    }
}
