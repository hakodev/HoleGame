using UnityEngine;
using Alteruna;

public class TurnNameTagTowardsPlayers : AttributesSync
{
    [SynchronizableField] int number;
    Alteruna.Avatar avatar;
    [SerializeField] Camera playerCamera;

    private void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
    }

    private void Update()
    {
        if(!avatar.IsMe) { return; }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }
    }
    private void Shoot()
    {
        if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, Mathf.Infinity))
        {
           ObjectObserver observer = hit.transform.gameObject.GetComponent<ObjectObserver>();
            if (observer!=null)
            {
                observer.ShotWithGun();
            }
            //invokeremotemethod 90(method, clientid)
        }
    }
}
