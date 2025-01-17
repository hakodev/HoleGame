using UnityEngine;
using Alteruna;
public class MousePainter : AttributesSync
{
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

    public void Start()
    {
        paintManager = FindAnyObjectByType<PaintManager>();
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
                Debug.Log(p.UID);
               // p.BroadcastRemoteMethod(nameof(p.SetTag), "drawnOn");
                BroadcastRemoteMethod(nameof(PaintOnAllClients),p.UID, hit.point, radius, hardness, strength, paintColor);
                //p.BroadcastRemoteMethod(nameof(p.ResetTag));
            }
        }
    }

    [SynchronizableMethod]
    private void PaintOnAllClients(Alteruna.IUniqueID PUID, Vector3 point, float radius, float hardness, float strength, UnityEngine.Color paintColor)
    {
        Paintable pop = Multiplayer.GetGameObjectById(PUID).GetComponent<Paintable>();   
        Debug.Log(pop.gameObject.name);
        PaintManager.Instance.paint(pop, point, radius, hardness, strength, paintColor);
    }
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Paint();
        }
    }

}