using TMPro;
using UnityEngine;

public class UIInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;


    public static string PlayerNameSync;


    public void ClickedJoinButton()
    {
        gameObject.SetActive(false);
    }
    public void ClickedStartButton()
    {
        gameObject.SetActive(false);
    }

    public void SetPlayerNameSync(string setName)
    {
        PlayerNameSync = setName;
    }
}
