using Alteruna;
using UnityEngine;

public class AudioListenerDisabler : AttributesSync
{
    Alteruna.Avatar avatar;

    private void Awake()
    {
        avatar = transform.root.gameObject.GetComponent<Alteruna.Avatar>();
    }

    void Start()
    {
        if(!avatar.IsMe)
        {
            GetComponent<AudioListener>().enabled = false;
            return;
        }
    }
}
