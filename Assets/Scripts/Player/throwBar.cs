using JetBrains.Annotations;
using System.Net.NetworkInformation;
using UnityEngine;

public class throwBar : MonoBehaviour
{
    public GameObject parentObject;
    public GameObject loadingBar;

    private float originalscalex;
    private void Start()
    {
        originalscalex = loadingBar.transform.localScale.x;       
        loadingBar.transform.localScale = new Vector3(0, loadingBar.transform.localScale.y, loadingBar.transform.localScale.z);
        parentObject.SetActive(false);
    }

    public void IncrementBar(float currentCharge, float maxCharge)
    {
        parentObject.SetActive(true);
        currentCharge = currentCharge / maxCharge;
        if(loadingBar.transform.localScale.x < originalscalex)
        {
            loadingBar.transform.localScale = new Vector3(currentCharge * originalscalex, loadingBar.transform.localScale.y, loadingBar.transform.localScale.z);
        }
    }

    public void ResetBar()
    {
        parentObject.SetActive(false);
        loadingBar.transform.localScale = new Vector3(0, loadingBar.transform.localScale.y, loadingBar.transform.localScale.z);
    }
}
