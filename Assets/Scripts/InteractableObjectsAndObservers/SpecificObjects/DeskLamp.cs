using UnityEngine;

public class DeskLamp : DynamicInteractableObject
{
    private Light[] spotLight;

    private Color currentColor;

    float timer = 0;

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

    [SynchronizableMethod]
    public override void Use()
    {
        currentColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        foreach (Light light in spotLight)
        {
            light.enabled = !light.enabled;
            light.color = currentColor;
        }

    }

    protected override void Update()
    {
        base.Update();
        /*timer += Time.deltaTime;
        if (timer > 0.5f)
        {
            timer = 0;
           
            foreach(Light light in spotLight)
            {
                
            }
        }*/
    }


}
