using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Alteruna;
using TMPro;
using System.IO;
using UnityEditor;
using System;
using Unity.VisualScripting;


public class PopUp : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private static GameObject roomMenu;
    private static GameObject roomCamera;

    [SerializeField] float popInTime;
    [SerializeField] float popOutTime;
    [SerializeField] float overPopImpact;
   
        [SerializeField] GameObject screenPopUpPrefab;
        [SerializeField] GameObject namePopUpPrefab;
        [SerializeField] GameObject colorPickerPrefab;
        UIInput uiInput;

        public GameObject screenObject;
        public Renderer screenRenderer;
        public RenderTexture screenRenderTexture;

    [SerializeField] bool triggersCaptcha = false;
    public void ToggleTriggerCaptcha(bool newState)
    {
        triggersCaptcha = newState;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

    }
    protected void Start()
    {

        if (roomMenu == null)
        {
            roomMenu = GameObject.FindGameObjectWithTag("RoomMenu");
            roomCamera = roomMenu.GetComponentInChildren<Camera>().gameObject;
            roomCamera.SetActive(false);
        }
        canvas = GetComponentInParent<Canvas>();
        uiInput = transform.root.GetComponentInChildren<UIInput>();
       
    }

    void OnEnable()
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



    public virtual void PopIn()
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
        screenPopUpPrefab.SetActive(true);
        colorPickerPrefab.SetActive(true);
        

    }

    TMP_InputField nameInputFieldText;
    public void ClickedVerifyNameButton()
    {


        nameInputFieldText = transform.parent.GetComponentInChildren<TMP_InputField>();

        if (nameInputFieldText.text == string.Empty)
        {
            GameObject noNamePopupPrefab = Resources.Load<GameObject>("PopupWrongName");
            GameObject captchaPopUp = Instantiate(noNamePopupPrefab, canvas.transform, false);
            captchaPopUp.GetComponent<RectTransform>().anchoredPosition = new Vector3(-286, 228, 0);
        }
        else
        {
            Texture screenTexture = screenRenderer.material.GetTexture("_MaskTexture");

            Texture2D texture2D = new Texture2D(1, 1, TextureFormat.RGBA32, false);

            RenderTexture currentRT = RenderTexture.active;

            RenderTexture renderTexture = new RenderTexture(1, 1, 1);
            Graphics.Blit(screenTexture, renderTexture);

            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(1, 1, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = currentRT;

            TexturesManager.Instance.width = 1;
            TexturesManager.Instance.height = 1;
            TexturesManager.Instance.SetTextureParams(texture2D.GetPixelData<Color32>(0).ToArray(), 1, 1);
            
            

            //RenderTexture.active = previous;
            //RenderTexture.ReleaseTemporary(screenRenderTexture);

            screenObject.SetActive(false);
            uiInput.SetPlayerNameSync(nameInputFieldText.text);
            roomCamera.SetActive(true);
            canvas.gameObject.SetActive(false);
          
        }

    }

    public virtual void ClickedScreenContinueButton()
    {
        namePopUpPrefab.SetActive(true);
        namePopUpPrefab.GetComponent<RectTransform>().anchoredPosition = new Vector3(-316, 188, 0);
    }
}
