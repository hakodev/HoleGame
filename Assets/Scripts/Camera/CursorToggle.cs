using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorToggle : MonoBehaviour
{
    private PlayerRole player;
    private Camera camera;
    List<Canvas> canvases;

    private layerOfUI Escape { get; set; } = layerOfUI.inGame;

    private void Awake()
    {
        camera = GetComponentInChildren<Camera>();
        canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        for (int i = 0; i < canvases.Count; i++)
        {
            if(canvases[i].gameObject.layer!=10) canvases.Remove(canvases[i]);
        }
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
         //   player = FindAnyObjectByType<PlayerRole>();
          //  if (player == null) { return; }

            if (Escape == layerOfUI.inGame && !performedAnAction)
            {
                performedAnAction = true;
                Escape = layerOfUI.interactWithPopUps;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }


            if (Escape == layerOfUI.interactWithPopUps && !performedAnAction)
            {
                performedAnAction = true;
                Escape = layerOfUI.inGame;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            performedAnAction = false;
        }
    }
    private void CheckForReturningToGame()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!HitUI())
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
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
