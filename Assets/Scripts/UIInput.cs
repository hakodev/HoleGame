using Alteruna;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TextMeshProUGUI invalidNameWarningText;
    [SerializeField] private GameObject roomMenu;

    public static string PlayerNameSync;

    public void ClickedPlayButton()
    {
        if (nameInputField.text == string.Empty)
        {
            invalidNameWarningText.DOKill();
            invalidNameWarningText.alpha = 1f;
            invalidNameWarningText.DOFade(0f, 3f);
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