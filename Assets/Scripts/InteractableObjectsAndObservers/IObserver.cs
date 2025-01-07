public interface IObserver
{
    public void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller);
}
public enum InteractionEnum
{
    ShotWithGun,
    Oiled,
    Watered,
    Caffenated,
    GrowthFormulaed
}