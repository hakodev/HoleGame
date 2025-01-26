using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Alteruna;
using TMPro;


public class PopUp : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private static GameObject roomMenu;

    [SerializeField] float popInTime;
    [SerializeField] float popOutTime;
    [SerializeField] float overPopImpact;

    [SerializeField] bool triggersCaptcha = false;

    UIInput uiInput;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        uiInput = transform.root.GetComponentInChildren<UIInput>();
        if (roomMenu == null)
        {
            roomMenu = GameObject.FindGameObjectWithTag("RoomMenu");
            roomMenu.SetActive(false);
        }
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



    public void PopIn()
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

    GameObject namePopUp;
    public void ClickedApplyButton()
    {
        if (namePopUp == null)
        {
            GameObject applyPopUpPrefab = Resources.Load<GameObject>("PopupEnterName");
            namePopUp = Instantiate(applyPopUpPrefab, canvas.transform, false);
            namePopUp.GetComponent<RectTransform>().anchoredPosition = new Vector3(-316, 188, 0);
        }
    }

    TMP_InputField nameInputFieldText;
    public void ClickedVerifyNameButton()
    {
        nameInputFieldText = GameObject.FindGameObjectWithTag("nameInput").GetComponent<TMP_InputField>();

        if (nameInputFieldText.text == string.Empty)
        {
            GameObject noNamePopupPrefab = Resources.Load<GameObject>("PopupWrongName");
            GameObject captchaPopUp = Instantiate(noNamePopupPrefab, canvas.transform, false);
            captchaPopUp.GetComponent<RectTransform>().anchoredPosition = new Vector3(-286, 228, 0);
        }
        else
        {
            uiInput.SetPlayerNameSync(nameInputFieldText.text);
            roomMenu.SetActive(true);
            canvas.gameObject.SetActive(false);
        }
    }
}
