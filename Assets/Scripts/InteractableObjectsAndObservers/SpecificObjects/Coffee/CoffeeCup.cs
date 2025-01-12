using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CoffeeCup : DynamicInteractableObject
{
    private GameObject coffeeFill;

    private bool coffeeFilled = false;

    private bool isThrown = false;

    private void Start()
    {
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
                new Vector3(coffeeFill.transform.localPosition.x, 1.1f, coffeeFill.transform.localPosition.z), t);
            t += Time.deltaTime * 0.3f;
            yield return new WaitForEndOfFrame();
        }

        coffeeFilled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isThrown && coffeeFilled)
        {
            Paintable p = collision.collider.GetComponent<Paintable>();
            if (p != null)
            {
                PaintManager.Instance.paint(p, transform.position, 0.8f, 0.5f, 0.5f, new Color(0.5f, 0.3f, 0.2f,1));
            }
        }
    }
}
