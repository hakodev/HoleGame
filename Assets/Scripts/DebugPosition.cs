using Alteruna;
using UnityEngine;

public class DebugPosition : MonoBehaviour
{
    Transform debugPositionTransform;
    Alteruna.Avatar avatar;

    private void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
    }

    private void Start()
    {
        debugPositionTransform = GameObject.FindGameObjectWithTag("DebugPosition")?.transform;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            if(avatar.IsMe)
            {
                transform.position = debugPositionTransform.position;
            }
        }
    }
}
