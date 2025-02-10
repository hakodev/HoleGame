using Alteruna;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows;
using static Unity.VisualScripting.Member;

public class DrawableScreenUI : PopUp, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    public void OnDrag(PointerEventData eventData)
    {
        //nothing
    }

    public new void ClickedVerifyNameButton()
    {
        base.ClickedScreenContinueButton();

        
    }
}
