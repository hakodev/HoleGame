using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class MousePainter : MonoBehaviour{
	public Camera cam;
	[Space]
	public bool mouseSingleClick;
	[Space]
	public Color paintColor;
	
	public float radius = 1;
	public float strength = 1;
	public float hardness = 1;

	public float range = 6;
	
	PaintManager paintManager;

	public LayerMask notPlayerMask;
	
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
            Paintable p = hit.collider.GetComponent<Paintable>();
            if (p != null)
            {
                PaintManager.Instance.paint(p, hit.point, radius, hardness, strength, paintColor);
            }
        }




    }

}