using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableLinks : MonoBehaviour, IPointerClickHandler
{
    TextMeshProUGUI tmp;
    [SerializeField] PopUp clippyMachines;
    [SerializeField] PopUp clippyInfiltrators;
    [SerializeField] PopUp clippySymptoms;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();  
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(tmp, eventData.position, null);
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = tmp.textInfo.linkInfo[linkIndex];

            if (linkInfo.GetLinkID() == "machinesID")
            {
                clippyMachines.gameObject.SetActive(true);
                clippyMachines.PopIn();
            }
            if (linkInfo.GetLinkID() == "infiltratorsID")
            {
                 clippyInfiltrators.gameObject.SetActive(true);
                clippyInfiltrators.PopIn();
            }
            if (linkInfo.GetLinkID() == "SymptomsID")
            {
                clippySymptoms.gameObject.SetActive(true);
                clippySymptoms.PopIn();
            }
        }
    }
    
    void OnDisable()
    {
        clippyMachines.PopOut();
        clippyInfiltrators.PopOut();
        clippySymptoms.PopOut();
    }
}
