using UnityEngine;

public class DeskLamp : DynamicInteractableObject
{
    private Light[] spotLight;

    protected override void Start()
    {
        base.Start();
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

    protected override void Update()
    {
        base.Update();
    }


}
