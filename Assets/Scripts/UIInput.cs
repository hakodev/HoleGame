using UnityEngine;

public class UIInput : MonoBehaviour
{
    [SerializeField] GameObject roomMenu;
    private void Awake()
    {

    }
    public void ClickedPlayButton()
    {
        roomMenu.SetActive(true);
        gameObject.SetActive(false);
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
