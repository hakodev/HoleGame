using UnityEngine;
using Alteruna;
using System.Collections.Generic;
using System.Linq;

public class AlarmsControl : AttributesSync
{
    private List<Alarm> allAlarms;
    [SynchronizableField] private bool isBlinking;

    [SerializeField] private AudioClip clip;
    [SerializeField] private float blinkSpeed;
    [SerializeField] private Color turnedOffColor;

    private float timer = 0;

    public float overallTimerGeneral;

    private bool isBlinkingUp;
    private bool isBlinkingDown;

    private bool isTurnedOn = false;
    private bool isBeeping = false;

    private int maxRounds;

    private CountDownDisplayManager countDownDisplayManager;


    private void Start()
    {
        countDownDisplayManager = FindAnyObjectByType<CountDownDisplayManager>();
        maxRounds = countDownDisplayManager.RoundsLeft;
        allAlarms = GetComponentsInChildren<Alarm>().ToList();

        foreach (Alarm alarm in allAlarms)
        {
            alarm.gameObject.GetComponent<Light>().enabled = false;
        }
    }

    [SynchronizableMethod]
    public void TurnOn()
    {
        foreach (Alarm alarm in allAlarms)
        {
            alarm.gameObject.GetComponent<Light>().enabled = true;
        }
        isTurnedOn = true;
    }

    [SynchronizableMethod]
    public void StartBlink()
    {
        isBlinking = true;
        isBlinkingUp = true;
    }

    private void Update()
    {
        
        if(CountDownDisplayManager.hasInitiatedTheTimer && !isTurnedOn)
        {
            TurnOn();
        }
        if((countDownDisplayManager.RoundsLeft / maxRounds < 0.4f) && !isBlinking)
        {
            StartBlink();
        }
        if ((countDownDisplayManager.RoundsLeft / maxRounds < 0.15f) && !isBeeping)
        {
            StartBeeping();
        }


        if (isBlinking)
        {
            foreach (Alarm alarm in allAlarms)
            {
                alarm.alarmLight.color = Color.Lerp(alarm.alarmLightColor, turnedOffColor, timer);
            }

            BlinkInterpolation();
            
        }
    }

    [SynchronizableMethod]
    private void StartBeeping()
    {
        foreach (Alarm alarm in allAlarms)
        {
            alarm.SetClip(clip);
        }
        isBeeping = true;
    }

    private void BlinkInterpolation()
    {
        if (isBlinkingUp)
        {
            timer += Time.deltaTime * blinkSpeed;

            if (timer > 1)
            {
                isBlinkingDown = true;
                isBlinkingUp = false;
            }
        }
        else if (isBlinkingDown)
        {
            timer -= Time.deltaTime * blinkSpeed;

            if (timer <= 0)
            {
                isBlinkingDown = false;
                isBlinkingUp = true;
            }
        }
    }

    [SynchronizableMethod]
    private void DisableFunctionality()
    {
        isBeeping = false;
        isBlinking = false;
        isTurnedOn = false;
        foreach(Alarm alarm in allAlarms)
        {
            alarm.isBeeping = false;
        }

        this.enabled = false;
    }

}
