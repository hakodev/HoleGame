using UnityEngine;
using Alteruna;
public class Gun : DynamicInteractableObject
{
    private Camera playerCamera;

    [SerializeField] int maxAmmo;
    [SynchronizableField] int currentAmmo;
    [SerializeField] float bulletMaxDistance;
    LayerMask otherPlayerLayerMask;
    public int Damage { get; set; }


    private void Start()
    {
        currentAmmo = maxAmmo;
        otherPlayerLayerMask = LayerMask.GetMask("PlayerLayer");
    }

    public override void SpecialInteraction(InteractionEnum interaction, UnityEngine.Component caller)
    {

    }
    public override void Use()
    {
        playerCamera = transform.root.Find("Camera").GetComponent<Camera>();
        Fire();
    }
    private void Fire()
    {
        // Debug.Log("Fired weapon");
        if (Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, bulletMaxDistance, otherPlayerLayerMask))
        {
            Debug.Log(hit.collider.gameObject.name + " " + transform.parent.gameObject.name);
            if (hit.collider.gameObject != transform.root.gameObject && hit.collider.gameObject.CompareTag("Player"))
            {
                // UserId targetUserId = hit.collider.gameObject.GetComponent<UserId>();
             //   hit.collider.gameObject.GetComponent<Health>().DamagePlayer(Random.Range(3, 7)); // Change later
                   Debug.Log("BULLSEYE!");
                hit.collider.gameObject.GetComponent<Interact>().SpecialInteraction(InteractionEnum.ShotWithGun, this);
            }
        }
        currentAmmo--;
    }
    private void Reload()
    {
        //  Debug.Log("Reloading...");
        currentAmmo = maxAmmo;
    }
}
