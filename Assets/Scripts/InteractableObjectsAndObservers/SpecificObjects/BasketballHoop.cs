using UnityEngine;

public class BasketballHoop : StationaryInteractableObject
{
    [SerializeField] BasketballTable table;
    public override void SpecialInteraction(InteractionEnum interaction, Component caller)
    {
        throw new System.NotImplementedException();
    }

    public override void Use()
    {
        throw new System.NotImplementedException();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Rigidbody>() != null)
        {
            table.BroadcastRemoteMethod(nameof(table.IncrementText));
            PlayerAudioManager.Instance.PlaySound(gameObject, source, PlayerAudioManager.Instance.GetBasketballBeep);
        }
    }
}
