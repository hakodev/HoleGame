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
        transform.DOScale(Vector3.one, popOutTime).SetEase(Ease.OutBack).OnComplete(() => transform.DOScale(Vector3.zero, popOutTime).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false))); 
    }

}
