using DG.Tweening;
using UnityEngine;

public class ClippyUI : MonoBehaviour
{
    [SerializeField] RectTransform officeRules;
    PopUp rulesPopUp;
    [SerializeField] float minHeight = 600;
    float maxHeight;

    bool isClippyOpen = false;

    private void Awake()
    {
        maxHeight = officeRules.rect.height;
        rulesPopUp = officeRules.GetComponent<PopUp>();
    }
    public void ClickedClippy()
    {
        if(!isClippyOpen)
        {
            isClippyOpen = true;
            rulesPopUp.gameObject.SetActive(true);
            // float targetHeight = officeRules.sizeDelta.y == maxHeight ? minHeight : maxHeight;
            // officeRules.DOSizeDelta(new Vector2(officeRules.sizeDelta.x, targetHeight), 0.5f).SetEase(Ease.OutCubic);

            rulesPopUp.PopIn();
        }
        else
        {
            isClippyOpen = false;
            rulesPopUp.PopOut();
        }
        // float targetHeight = officeRules.sizeDelta.y == minHeight ? maxHeight : minHeight;
        // officeRules.DOSizeDelta(new Vector2(officeRules.sizeDelta.x, targetHeight), 0.5f).SetEase(Ease.OutCubic).OnComplete(() => rulesPopUp.PopOut());
    }
}

