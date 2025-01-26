using TMPro;
using UnityEngine;
using Alteruna;


public class CountdownDisplay : AttributesSync {

    [SerializeField] private int secondsRemainingToTurnRed;
    [SerializeField] TextMeshProUGUI countdown;
    TextMeshProUGUI flavorTextMesh;
    [SerializeField] private CountDownDisplayManager manager;

    [SynchronizableField] public int time;
    [SynchronizableField] public static Color countdownColor = Color.green;
    private float deltaTime=0;
    public int maxTime;


    public static int sendTimeToUI;
    public static string sendFlavorTextToUI;


    private void Awake() {
        maxTime = time;
        flavorTextMesh = transform.Find("CountdownPrefix").GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        //flavorTextMesh = transform.Find("CountdownPrefix").GetComponent<TextMeshProUGUI>();
        //sendFlavorTextToUI = flavorTextMesh.text;
    }
    

    [SynchronizableMethod]
    private void DeactivateUnusedTimers()//(string deactivatedObject)
    {
        gameObject.SetActive(false);
    }

    [SynchronizableMethod]
    private void InitiateVotingPhaseForAllPlayers()
    {
        VotingPhase[] allVotingPhases = FindObjectsByType<VotingPhase>(FindObjectsSortMode.None);
        foreach (VotingPhase player in allVotingPhases)
        {
           // Debug.Log(player.gameObject.name);
            player.InitiateVotingPhase();
        }
    }
    [SynchronizableMethod]
    private void EndVotingPhaseForAllPlayers()
    {
        //VotingPhase[] allVotingPhases = FindObjectsByType<VotingPhase>(FindObjectsSortMode.None);
        VotingPhase player = Multiplayer.GetAvatar().gameObject.GetComponent<VotingPhase>();
        //Debug.Log(player.gameObject.name);
        player.EndVotingPhase();
        


        SymptomNotifText[] allNotifTexts = FindObjectsByType<SymptomNotifText>(FindObjectsSortMode.None);
        foreach(SymptomNotifText notifText in allNotifTexts)
        {
            // This will enable the notification canvas for all players
            notifText.transform.parent.parent.gameObject.SetActive(true);
        }
    }

    private void Update() {
        if (CountDownDisplayManager.hasInitiatedTheTimer)
        {
            UpdateTickDown();
            UpdateUI();
            sendTimeToUI = time;
        }
    }

    
    private void UpdateUI()
    {
        countdown.text = time.ToString();

        countdownColor = time <= secondsRemainingToTurnRed ? Color.red : Color.green;
        countdown.color = countdownColor;
    }

    
    private void UpdateTickDown()
    {
        if (!Multiplayer.GetUser().IsHost) { return; }
        if (!Multiplayer.GetAvatar().IsMe) { return; }
        if (RoleAssignment.playerID - 1 != 0) { return; }


        if (time > 0) 
        {
            deltaTime += Time.deltaTime;
            if (deltaTime >= 1)
            {
                deltaTime = 0;
                if (Multiplayer.GetUser().IsHost)
                {
                    time--;
                    manager.TimeToEndTheGame--;
                }
                //Debug.Log(gameObject.name);
            }
        }
        else
        {
            manager.BroadcastRemoteMethod("ActivateTimer", parameters: gameObject.name);
            BroadcastRemoteMethod(nameof(DeactivateUnusedTimers));
            
            if(gameObject.CompareTag("DowntimeDisplay")) BroadcastRemoteMethod(nameof(InitiateVotingPhaseForAllPlayers));
            if (gameObject.CompareTag("VotingDisplay")) BroadcastRemoteMethod(nameof(EndVotingPhaseForAllPlayers));
        }
    }
}
