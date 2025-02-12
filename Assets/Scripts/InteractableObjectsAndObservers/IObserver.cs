public interface IObserver
{
    public void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller);
}
public enum InteractionEnum
{
    ShotWithGun,
    PlacedStickyNote,
    ThrownStickyNote,
    PickedUpStickyNote,
    GivenTaskManagerRole,
    RemoveGun,
    PlaceCupInCoffeeMachine,
    MarkerOnPosterOrStickyNote,
    StoppedMarkerOnPosterOrStickyNote,
    CoffeeStain,
    smackedPlayerWithObject
}