using Unity.Netcode;
using UnityEngine;

public class JoinUI : MonoBehaviour
{
    [SerializeField] GameObject joinUICanvas;


    public void ExitUI()
    {
        //fancy camera rotating
        //animation of UI switching off
        joinUICanvas.SetActive(false);
    }
}
