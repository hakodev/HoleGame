using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CoffeeDripObject : MonoBehaviour
{
    private Material material;

    public float finalThreshold;

    public Vector2 startScale;
    public Vector2 endScale;



    private void Start()
    {
        material = GetComponent<Renderer>().material;
        
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            EnableDrip();
        }
    }

    public void EnableDrip()
    {
        StartCoroutine(LerpDrip());
    }

    public void OnDisable()
    {

    }

    public IEnumerator LerpDrip()
    {
        float startThreshold = material.GetFloat("_CoffeeNoiseThreshold");

        float t = 0;

        while (t < 1)
        {
            material.SetFloat("_CoffeeNoiseThreshold", Mathf.Lerp(startThreshold, finalThreshold, t));
            transform.localScale = Vector3.Lerp(new Vector3(startScale.x, startScale.y, transform.localScale.z), new Vector3(endScale.x, endScale.y, transform.localScale.z), t);
            t += Time.deltaTime * 0.6f;
            yield return new WaitForEndOfFrame();
        }
    }
}
