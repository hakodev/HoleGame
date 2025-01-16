using UnityEngine;

public class Marker : DynamicInteractableObject
{
    public MousePainter painter;

    public override void SpecialInteraction(InteractionEnum interaction, Component caller)
    {

    }

    public override void Use()
    {

    }

    private void Update()
    {
        if (Input.GetMouseButton(1) && isPickedUp) 
        {
            if (painter == null)
            {
                painter = GetComponent<MousePainter>();
            }
            if (!painter.cam)
            {
                painter.cam =transform.root.GetComponentInChildren<Camera>();
            }
            painter.Paint();
        }
    }

}
