using Unity.VisualScripting;
using UnityEngine;

public class Alarm : MonoBehaviour
{
    //[HideInInspector] public float startIntensity;
    [HideInInspector] public Light alarmLight;
    [HideInInspector] public Color alarmLightColor;
    [HideInInspector] public bool isBeeping;

    AudioClip alarmClip;
    AudioSource alarmSource;

    private float beepTimer = 0;

    void Start()
    {
        alarmLight = GetComponent<Light>();
        alarmLightColor = alarmLight.color;

    }

    void Update()
    {
        if (isBeeping)
        {
            beepTimer += Time.deltaTime;
            if(beepTimer > 1.2f)
            {
                beepTimer = 0;
                alarmSource.Play();
            }
        }

    }

    public void SetClip(AudioClip clip)
    {
        alarmClip = clip;

        EnableSource();
    }

    void EnableSource()
    {
        alarmSource = gameObject.AddComponent<AudioSource>();
        alarmSource.clip = alarmClip;
        alarmSource.spatialBlend = 0.85f;
        alarmSource.volume = 0.1f;

        AudioReverbFilter filter = alarmSource.AddComponent<AudioReverbFilter>();
        filter.reverbPreset = AudioReverbPreset.Room;

        isBeeping = true;
    }
}
