using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;

public class CoffeeCup : DynamicInteractableObject
{
    private GameObject coffeeFill;

    public bool coffeeFilled = false;

    private bool isThrown = false;

    AudioSource hitSource;

    public float coffeeFillHeight;


    protected override void Start()
    {
        base.Start();
        coffeeFill = transform.GetChild(0).gameObject;
    }
    public override void SpecialInteraction(InteractionEnum interaction, Component caller)
    {
        isThrown = true;
    }

    public override void Use()
    {
        
    }

    public IEnumerator FillCoffee()
    {
        float t = 0;
        while (t < 1)
        {
            coffeeFill.transform.localPosition = Vector3.Lerp(new Vector3(coffeeFill.transform.localPosition.x, 0, coffeeFill.transform.localPosition.z),
                new Vector3(coffeeFill.transform.localPosition.x, coffeeFillHeight, coffeeFill.transform.localPosition.z), t);
            t += Time.deltaTime * 0.3f;
            yield return new WaitForEndOfFrame();
        }

        coffeeFilled = true;
    }

    protected override void OnCollisionEnter(Collision info)
    {
        //base.OnCollisionEnter(info);
        // find collision point and normal. You may want to average over all contacts
        var point = info.contacts[0].point;
        var dir = -info.contacts[0].normal; // you need vector pointing TOWARDS the collision, not away from it

        Vector3 normal = new Vector3();// step back a bit
        float angle = 0;
        point -= dir;
        RaycastHit hitInfo;
        // cast a ray twice as far as your step back. This seems to work in all
        // situations, at least when speeds are not ridiculously big
        Vector3 pos = point - (-dir * 0.1f);
        
        if (info.collider.Raycast(new Ray(point, dir), out hitInfo, 2))
        {
            // this is the collider surface normal
            normal = hitInfo.normal;
            // this is the collision angle
            // you might want to use .velocity instead of .forward here, but it 
            // looks like it's already changed due to bounce in OnCollisionEnter
            angle = Vector3.Angle(-transform.forward, normal);
        }
        if (isThrown && coffeeFilled)
        {
            if (((1 << info.gameObject.layer) & PaintManager.Instance.mask) == 0) return;

            BroadcastRemoteMethod(nameof(CreateDecalPrefab), pos, normal);
        }

        PlayerAudioManager.Instance.PlaySound(gameObject, hitSource, PlayerAudioManager.Instance.GetMug);
    }

    [SynchronizableMethod]
    void CreateDecalPrefab(Vector3 pos, Vector3 normal)
    {
        Instantiate(PaintManager.Instance.GetStain(), pos, Quaternion.LookRotation(-normal));
    }
}
