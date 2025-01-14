using UnityEngine;

public class SelfCanvas : MonoBehaviour
{
    Alteruna.Avatar avatar;

    private void Awake()
    {
        avatar = transform.root.GetComponent<Alteruna.Avatar>();
    }
    private void Start()
    {
        if(!avatar.IsMe)
        {
            gameObject.SetActive(false);
        }
    }
}
