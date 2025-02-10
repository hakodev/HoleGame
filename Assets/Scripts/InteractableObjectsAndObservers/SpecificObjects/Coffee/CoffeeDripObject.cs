using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CoffeeDripObject : MonoBehaviour
{
    private Material material;

    public float finalThreshold;
    float startThreshold;

    public Vector2 startScale;
    public Vector2 endScale;

    [HideInInspector]
    public bool isDripping = false;

    private void Start()
    {
        material = GetComponent<Renderer>().material;
        startThreshold = material.GetFloat("_CoffeeNoiseThreshold");
    }

    public void EnableDrip()
    {
        StartCoroutine(LerpDripUp());
    }

    public void DisableDrip()
    {
        StartCoroutine(LerpDripDown());
    }

    public IEnumerator LerpDripUp()
    {
        isDripping = true;

        float t = 0;

        while (t < 1)
        {
            material.SetFloat("_CoffeeNoiseThreshold", Mathf.Lerp(startThreshold, finalThreshold, t));
            transform.localScale = Vector3.Lerp(new Vector3(startScale.x, startScale.y, transform.localScale.z), new Vector3(endScale.x, endScale.y, transform.localScale.z), t);
            t += Time.deltaTime * 0.6f;
            yield return new WaitForEndOfFrame();
        }

        Invoke("DisableDrip",2);

    }

    public IEnumerator LerpDripDown()
    {
        float t = 0;

        while (t < 1)
        {
            material.SetFloat("_CoffeeNoiseThreshold", Mathf.Lerp(finalThreshold, 0.5f, t));
            transform.localScale = Vector3.Lerp(new Vector3(endScale.x, endScale.y, transform.localScale.z), new Vector3(0,0, transform.localScale.z), t);
            t += Time.deltaTime * 0.6f;
            yield return new WaitForEndOfFrame();
        }

        isDripping = false;
    }
}
