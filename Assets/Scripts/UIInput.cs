using TMPro;
using UnityEngine;

public class UIInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;
    private Canvas canvas;
    private GameObject roomMenu;

    public static string PlayerNameSync;

    public void ClickedPlayButton()
    {
        roomMenu = GameObject.FindGameObjectWithTag("RoomMenu");
        canvas = GetComponentInParent<Canvas>();

        if (nameInputField.text == string.Empty) {
            GameObject noNamePopupPrefab = Resources.Load<GameObject>("PopupWrongName");
            GameObject captchaPopUp = Instantiate(noNamePopupPrefab, canvas.transform, false);
            captchaPopUp.GetComponent<RectTransform>().anchoredPosition = new Vector3(-286, 228, 0);
        } else { 
            PlayerNameSync = nameInputField.text;
            Debug.Log("1 " + PlayerNameSync);
            roomMenu.SetActive(true);
            gameObject.SetActive(false);
        }
    }
    public void ClickedJoinButton()
    {
        gameObject.SetActive(false);
    }
    public void ClickedStartButton()
    {
        gameObject.SetActive(false);
    }
}
