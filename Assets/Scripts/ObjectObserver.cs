using Alteruna;
using UnityEngine;

public class ObjectObserver : AttributesSync
{
    public void ShotWithGun()
    {
        Debug.Log(gameObject.name + " shot with a gun");
    }
}
