using UnityEngine;
using Alteruna;
using System;
public class MousePainter : MonoBehaviour {

    public Camera cam;
    [Space]
    public UnityEngine.Color paintColor;

    public float radius = 1;
    public float strength = 1;
    public float hardness = 1;

    public float range = 6;

    PaintManager paintManager;

    public LayerMask notPlayerMask;
    Paintable p;

    public bool isFreeForm = false;

    public void Start()
    {
        paintManager = FindAnyObjectByType<PaintManager>();
    }

    public void Paint(Camera cam)
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

                paintManager.BroadcastRemoteMethod("paint", id, hit.point, radius, paintColor);
               
            }
        }

    }

    private void Update()
    {

        if (isFreeForm && Input.GetMouseButton(0))
        {
            Paint(cam);
        }
        
    }

    public void ChangeColor(UnityEngine.Color color)
    {
        paintColor = color;
    }

}