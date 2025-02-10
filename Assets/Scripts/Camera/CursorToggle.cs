using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorToggle : MonoBehaviour
{
    private CameraMovement camMovement;
    List<Canvas> canvases;

    private layerOfUI Escape { get; set; } = layerOfUI.inGame;

    bool once = true;

    private void Start()
    {
        canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        camMovement = GetComponentInChildren<CameraMovement>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        CheckForEsc();
        CheckForReturningToGame();
    }

    bool performedAnAction = false;
    private void CheckForEsc()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Escape == layerOfUI.inGame && !performedAnAction && !StickyNote.currentlyDrawing)
            {
                performedAnAction = true;
                Escape = layerOfUI.interactWithPopUps;
                UICursorAndCam(true);
            }


            if (Escape == layerOfUI.interactWithPopUps && !performedAnAction && !StickyNote.currentlyDrawing)
            {
                performedAnAction = true;
                Escape = layerOfUI.inGame;
                UICursorAndCam(false);
            }

            performedAnAction = false;
        }
    }


    private void CheckForReturningToGame()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (once)
            {
                once = false;
                for (int i = 0; i < canvases.Count; i++)
                {
                    if (canvases[i].gameObject.layer != 10) canvases.Remove(canvases[i]);
                }
            }

            if (!HitUI())
            {
                if(!StickyNote.currentlyDrawing)
                {
                    UICursorAndCam(true);
                }
            }
        }
    }

    public void UICursorAndCam(bool enterUIMode)
    {
        if (enterUIMode)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            camMovement.FreezeCameraRotation = true;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            camMovement.FreezeCameraRotation = false;
        }
    }

    private bool HitUI()
    {
        EventSystem eventSystem = EventSystem.current;
        PointerEventData pointerEventData = new PointerEventData(eventSystem){position = Input.mousePosition};
        List<RaycastResult> raycastResults = new List<RaycastResult>();

        int hitCount = 0;
        for(int i=0; i<canvases.Count; i++)
        {
            if (canvases[i] == null) {  continue; }

            GraphicRaycaster raycaster = canvases[i].GetComponent<GraphicRaycaster>();
            raycaster.Raycast(pointerEventData, raycastResults);

            foreach (RaycastResult result in raycastResults)
            {
                hitCount++;
                Debug.Log("hitCanvas " + canvases[i]);
            }
        }
        return hitCount > 0;
    }
}

public enum layerOfUI
{
    inGame,
    interactWithPopUps
}
