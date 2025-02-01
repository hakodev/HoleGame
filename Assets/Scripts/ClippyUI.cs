using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ClippyUI : MonoBehaviour
{
    [SerializeField] RectTransform officeRules;
    PopUp rulesPopUp;
    [SerializeField] float minHeight = 600;
    float maxHeight;

    bool isClippyOpen = false;
    bool hasBeenInteractedWith = false;

    private Color startColor;
    public Color blinkColor;

    float time = 0;

    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        startColor = image.color;
        maxHeight = officeRules.rect.height;
        rulesPopUp = officeRules.GetComponent<PopUp>();
    }
    public void ClickedClippy()
    {
        if(!isClippyOpen)
        {
            hasBeenInteractedWith = true;
            image.color = startColor;
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

    private void Update()
    {
        if (!hasBeenInteractedWith)
        {
            time = Mathf.Abs(Mathf.Sin(Time.time * 2));
            Debug.Log(time);
            image.color = Color.Lerp(startColor, blinkColor, time);
        }
    }

    
}

