public interface IObserver
{
    public void SpecialInteraction(InteractionEnum interaction);
}
public enum InteractionEnum
{
    ShotWithGun,
    Oiled,
    Watered,
    Caffenated,
    GrowthFormulaed
}