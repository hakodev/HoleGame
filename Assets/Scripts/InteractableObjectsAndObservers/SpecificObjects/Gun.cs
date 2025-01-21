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

   // Animator playerAnimator;
   // AnimationSynchronizable playerAnimatorSync;
    public int Damage() {
        Debug.Log(damage);
        return damage;
    }

    protected override void Start()
    {
        base.Start();
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
        PlayerAudioManager.Instance.PlaySound(this.gameObject, PlayerAudioManager.Instance.GetLaserBeep);

        if (Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, bulletMaxDistance, otherPlayerLayerMask))
        {
            Debug.Log(hit.collider.gameObject.name + " " + transform.parent.gameObject.name);
            if (hit.collider.gameObject != transform.root.gameObject && hit.collider.gameObject.CompareTag("Player"))
            {
                hit.collider.gameObject.GetComponent<Interact>().SpecialInteraction(InteractionEnum.ShotWithGun, this);
                transform.root.gameObject.GetComponent<Interact>().SpecialInteraction(InteractionEnum.RemoveGun, this);
                currentAmmo--;
            }
        }
        
    }
}
