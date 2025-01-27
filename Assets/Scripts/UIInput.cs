using Alteruna;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInput : MonoBehaviour
{
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

