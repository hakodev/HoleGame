using Unity.Netcode;
using UnityEngine;

public class JoinUI : MonoBehaviour
{
    [SerializeField] GameObject joinUICanvas;

    private void Awake()
    {
        
    }
    public void HostButtonPressed()
    {
        NetworkManager.Singleton.StartHost();
        HideJoinUICanvas();
    }
    public void ServerButtonPressed()
    {
        NetworkManager.Singleton.StartServer();
        HideJoinUICanvas();
    }
    public void ClientButtonPressed()
    {
        NetworkManager.Singleton.StartClient();
        HideJoinUICanvas();
    }

    private void HideJoinUICanvas()
    {
        joinUICanvas.SetActive(false);
    }
}
