using UnityEngine;

public class UIInput : MonoBehaviour
{
    GameObject lobbyManager;
    private void Awake()
    {
        lobbyManager = GameObject.Find("LobbyManager");
        lobbyManager.SetActive(false);
    }
    public void ClickedPlayButton()
    {
        lobbyManager.SetActive(true);
        gameObject.SetActive(false);
    }
}
