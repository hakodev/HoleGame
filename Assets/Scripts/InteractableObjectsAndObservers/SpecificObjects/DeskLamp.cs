using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class DeskLamp : DynamicInteractableObject
{
    private Light[] spotLight;

    private void Start()
    {
        spotLight = GetComponentsInChildren<Light>();
        foreach (Light light in spotLight)
        {
            light.enabled = false;
        }
    }

    public override void SpecialInteraction(InteractionEnum interaction, Component caller)
    {
       
    }

    public override void Use()
    {
        foreach (Light light in spotLight)
        {
            light.enabled = !light.enabled;
        }

    }

    private void Update()
    {
        
    }


}
