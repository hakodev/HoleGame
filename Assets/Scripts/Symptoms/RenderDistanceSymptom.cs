using Alteruna;
using System.Threading;
using UnityEngine;

public class RenderDistanceSymptom : MonoBehaviour
{
    public bool isEnabled = true;
    float renderDistanceTimer = 2;

    private Transform[] spheres;

    Alteruna.Avatar avatar;

    private void Start()
    {
        avatar = transform.root.GetComponent<Alteruna.Avatar>();
        spheres = new Transform[3];
        SymptomsManager.Instance.SetRenderDistanceSymptom(this);

        for (int i = 0; i < transform.childCount; i++) 
        {
            spheres[i] = transform.GetChild(i);
        }
        foreach (Transform sphere in spheres)
        {
            sphere.gameObject.SetActive(false);
        }
    }
    public void SetRenderDistance()
    {
        if (!avatar.IsMe) return;
        gameObject.layer = 0;
        foreach (Transform sphere in spheres)
        {
            sphere.gameObject.layer = 0;
            sphere.gameObject.SetActive(true);
        }
    }

    public void SetRenderDistanceBack()
    {
        foreach (Transform sphere in spheres)
        {
            sphere.gameObject.SetActive(false);
        }
    }

    public void SetEnabled()
    {
        isEnabled = !isEnabled;
    }

    private void Update()
    {
        if (isEnabled)
        {
            renderDistanceTimer += Time.deltaTime;

            if (renderDistanceTimer > 2)
            {
                SetRenderDistance();
                renderDistanceTimer = 0;
            }
        }
    }
}
