using UnityEngine;

public class Gun : DynamicInteractableObject
{
    [SerializeField] private Camera playerCamera; 
    public override void SpecialInteraction(InteractionEnum interaction)
    {

    }
    public override void Use()
    {
        RaycastHit hit;
        if(Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit, 40))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                hit.collider.gameObject.GetComponent<Interact>().SpecialInteraction(InteractionEnum.ShotWithGun);
            }
        }
    }
}
