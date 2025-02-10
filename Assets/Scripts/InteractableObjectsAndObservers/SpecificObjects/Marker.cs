using UnityEngine;

public class Marker : DynamicInteractableObject
{
    public MousePainter painter;
    Camera cam;
    [SynchronizableField]
    private int userID;
    Interact interact;
    HUDDisplay display;

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
            ProcessMarkerOnStickyNote();    
            painter.Paint(cam);
        }
    }

    /*private new void LateUpdate()
    {
        if (isPickedUp)
        {
            if (display == null || display.gameObject.transform.root != transform.root)
            {
                display = transform.root.GetComponentInChildren<HUDDisplay>();
            }
            display.SetState(new MarkerDisplay(display));
        }
    }*/

    private void ProcessMarkerOnStickyNote()
    {
        if (Input.GetMouseButtonDown(1))
        {
            interact = currentlyOwnedByAvatar.gameObject.GetComponent<Interact>();

            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(new Vector2(cam.pixelWidth / 2, cam.pixelHeight / 2)), out hit, interact.GetGrabReach()))
            {
                StickyNote s = hit.transform.GetComponent<StickyNote>();
                if (s != null)
                {
                    s.SpecialInteraction(InteractionEnum.MarkerOnPosterOrStickyNote, this);
                }
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(new Vector2(cam.pixelWidth / 2, cam.pixelHeight / 2)), out hit, interact.GetGrabReach()))
            {
                StickyNote s = hit.transform.GetComponent<StickyNote>();
                if (s != null)
                {
                    s.SpecialInteraction(InteractionEnum.StoppedMarkerOnPosterOrStickyNote, this);
                }
            }
        }
    }

}
