using UnityEngine;
using System.Collections;
public class Gun : DynamicInteractableObject
{
    private Camera playerCamera;

    [SerializeField] int maxAmmo;
    [SynchronizableField] int currentAmmo;
    [SerializeField] float bulletMaxDistance;
    LayerMask otherPlayerLayerMask; //everything but the self player layer
    [SynchronizableField] [SerializeField] int damage;

    public Transform recoilRotationTransform;
    Quaternion startRotation;
    Transform childTransform;
    ParticleSystem glitchParticle;
    

    AudioSource source;
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
        otherPlayerLayerMask = ~(1 << 10);
        childTransform = transform.GetChild(0);

        startRotation = childTransform.localRotation;
        glitchParticle = GetComponentInChildren<ParticleSystem>();

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
        PlayerAudioManager.Instance.PlaySound(this.gameObject, source, PlayerAudioManager.Instance.GetLaserBeep);
        StartCoroutine(Recoil(childTransform.localRotation));
        glitchParticle.Play();

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
    
    private IEnumerator Recoil(Quaternion startRotation)
    {
        float t = 0;

        while (t < 1)
        {
            childTransform.localRotation = Quaternion.Lerp(startRotation, recoilRotationTransform.localRotation, t);

            t += Time.deltaTime * 5;

            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(RecoilBack());
    }

    private IEnumerator RecoilBack()
    {
        float t = 0;

        while (t < 1)
        {
            childTransform.localRotation = Quaternion.Lerp(recoilRotationTransform.localRotation, startRotation, t);

            t += Time.deltaTime * 5;

            yield return new WaitForEndOfFrame();
        }
    }
}
