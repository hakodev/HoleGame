
public abstract class HUDState
{
    public HUDDisplay display;
    public HUDState(HUDDisplay displayToSet)
    {
        display = displayToSet;
    }
    public abstract void Display();
}
