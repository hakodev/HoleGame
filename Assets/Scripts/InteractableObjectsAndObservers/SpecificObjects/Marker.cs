using UnityEngine;

public class Marker : DynamicInteractableObject
{
    public MousePainter painter;
    Camera cam;

    private void Start()
    {
        base.Start(); 
        painter = GetComponent<MousePainter>();
    } 

    public override void SpecialInteraction(InteractionEnum interaction, Component caller)
    {

    }

    public override void Use()
    {

    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetMouseButton(1) && isPickedUp) 
        {
            if (cam == null)
            {
                cam = transform.root.GetComponentInChildren<Camera>();
            }
            painter.Paint(cam);
        }
    }

}
