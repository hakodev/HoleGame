using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;


public class PopUp : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    [SerializeField] float popInTime;
    [SerializeField] float popOutTime;
    [SerializeField] float overPopImpact;

    [SerializeField] bool triggersCaptcha = false;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }
    private void Start()
    {
        PopIn();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }



    private void PopIn()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(overPopImpact, popInTime).SetEase(Ease.OutBack).OnComplete(() => transform.DOScale(Vector3.one, popInTime).SetEase(Ease.OutBack));
    }

    public void PopOut()
    {
        if (!triggersCaptcha)
        {
            transform.DOScale(Vector3.one, popOutTime).SetEase(Ease.OutBack).OnComplete(() => transform.DOScale(Vector3.zero, popOutTime).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false)));
        }
        else
        {
            GameObject captchaPopUpPrefab = Resources.Load<GameObject>("PopupCaptcha");
            GameObject captchaPopUp = Instantiate(captchaPopUpPrefab, canvas.transform, false);
            captchaPopUp.GetComponent<RectTransform>().anchoredPosition = new Vector3(453, -8, 0);
        }
    }

    GameObject applyPopUp;
    public void ClickedApplyButton()
    {
        if (applyPopUp == null)
        {
            GameObject applyPopUpPrefab = Resources.Load<GameObject>("PopupApply");
            GameObject applyPopUp = Instantiate(applyPopUpPrefab, canvas.transform, false);
            applyPopUp.GetComponent<RectTransform>().anchoredPosition = new Vector3(-316, 188, 0);
        }
    }
}
