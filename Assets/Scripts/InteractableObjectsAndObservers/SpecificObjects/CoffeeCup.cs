using System.Collections;
using UnityEngine;

public class CoffeeCup : DynamicInteractableObject
{
    private GameObject coffeeFill;

    private bool coffeeFilled = false;

    private void Start()
    {
        coffeeFill = transform.GetChild(0).gameObject;
    }
    public override void SpecialInteraction(InteractionEnum interaction, Component caller)
    {

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
                new Vector3(coffeeFill.transform.localPosition.x, 1.2f, coffeeFill.transform.localPosition.z), t);
            t += Time.deltaTime * 0.3f;
            yield return new WaitForEndOfFrame();
        }

        coffeeFilled = true;
    }

}
