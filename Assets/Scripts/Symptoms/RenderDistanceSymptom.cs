using UnityEngine;

public class RenderDistanceSymptom : MonoBehaviour
{
    float renderDistanceTimer = 2;

    private Transform[] spheres;

    Alteruna.Avatar avatar;

    private bool isInactive;

    private PlayerRole playerRole;

    private void Start()
    {
        avatar = transform.root.GetComponent<Alteruna.Avatar>();
        playerRole = transform.root.GetComponent<PlayerRole>();
        spheres = new Transform[3];

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
        isInactive = false;
    }

    public void SetRenderDistanceBack()
    {
        foreach (Transform sphere in spheres)
        {
            sphere.gameObject.SetActive(false);
        }
        isInactive = true;
    }

    private void Update()
    {
        if (SymptomsManager.Instance.GetSymptom() == SymptomsManager.Instance.GetSymptomsList()[2] && playerRole.GetRole()==Roles.Machine)
        {

            SetRenderDistance();
        }
        else
        {
            SetRenderDistanceBack();
        }
    }
}
