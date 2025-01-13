using UnityEngine;
using Alteruna;
public class Gun : DynamicInteractableObject
{
    private Camera playerCamera;

    [SerializeField] int maxAmmo;
    [SynchronizableField] int currentAmmo;
    [SerializeField] float bulletMaxDistance;
    LayerMask otherPlayerLayerMask;
    [SynchronizableField] [SerializeField] int damage;

    MishSyncAnimations playerAnim;
    public int Damage() {
        Debug.Log(damage);
        return damage;
    }

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
        playerAnim = transform.root.GetComponent<MishSyncAnimations>();
        Fire();
    }
    private void Fire()
    {
        if (Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, bulletMaxDistance, otherPlayerLayerMask))
        {
            Debug.Log(hit.collider.gameObject.name + " " + transform.parent.gameObject.name);
            if (hit.collider.gameObject != transform.root.gameObject && hit.collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("BULLSEYE!");
                hit.collider.gameObject.GetComponent<Interact>().SpecialInteraction(InteractionEnum.ShotWithGun, this);
                playerAnim.SetShooting(true);
            }
        }
        currentAmmo--;
        playerAnim.SetShooting(true);
    }
    private void Reload()
    {
        currentAmmo = maxAmmo;
    }
}
