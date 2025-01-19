using UnityEngine;
using Alteruna;
using System;
using System.Drawing;
public class MousePainter : MonoBehaviour {

    public Camera cam;
    [Space]
    public bool mouseSingleClick;
    [Space]
    public UnityEngine.Color paintColor;

    public float radius = 1;
    public float strength = 1;
    public float hardness = 1;

    public float range = 6;

    PaintManager paintManager;

    public LayerMask notPlayerMask;
    Paintable p;
    Alteruna.Avatar avatar;

    public void Start()
    {
        paintManager = FindAnyObjectByType<PaintManager>();
        avatar = transform.root.GetComponent<Alteruna.Avatar>();
    }

    public void Paint()
    {
        Vector3 position = Input.mousePosition;
        Ray ray = cam.ScreenPointToRay(position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, notPlayerMask))
        {
            p = hit.collider.GetComponent<Paintable>();
            if (p != null)
            {
                CommunicationBridgeUID puid = p.GetComponent<CommunicationBridgeUID>();
                Guid id = puid.GetUID();

                paintManager.BroadcastRemoteMethod("paint", id, hit.point, radius, hardness, strength, paintColor);
                
            }
        }

    }

    private void Update()
    {
        if (!avatar.IsMe) { return; }
        if (Input.GetMouseButton(0))
        {
            Paint();
        }
    }

}