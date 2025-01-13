using UnityEngine;

public abstract class HUDState : MonoBehaviour
{
    public HUDDisplay display;
    public HUDState(HUDDisplay displayToSet)
    {
        display = displayToSet;
    }
    public abstract void Display();
}
