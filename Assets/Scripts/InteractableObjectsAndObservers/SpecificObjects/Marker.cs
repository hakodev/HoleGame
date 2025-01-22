using UnityEngine;

public class Marker : DynamicInteractableObject
{
    public MousePainter painter;
    Camera cam;
    [SynchronizableField]
    private int userID;

    protected override void Start()
    {
        base.Start(); 
        painter = GetComponent<MousePainter>();
    } 

    public override void SpecialInteraction(InteractionEnum interaction, Component caller)
    {

    }

    public override void Use()
    {
        userID = Multiplayer.GetUser().Index;
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetMouseButton(1) && isPickedUp) 
        {
            if (userID != Multiplayer.GetUser().Index) { return; }
            if (cam == null)
            {
                cam = transform.root.GetComponentInChildren<Camera>();
            }
            RaycastHit hit;
           // if()

            painter.Paint(cam);
        }
    }

}
