using UnityEngine.EventSystems;


public class DrawableScreenUI : PopUp, IDragHandler, IBeginDragHandler, IEndDragHandler
{



    public new void ClickedVerifyNameButton()
    {
        base.ClickedScreenContinueButton();

        
    }
}
