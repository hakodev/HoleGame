using TMPro;
using UnityEngine;
using Alteruna;


public class CountdownDisplay : AttributesSync {

    [SerializeField] private int secondsRemainingToTurnRed;
    [SerializeField] TextMeshProUGUI countdown;
    [SerializeField] TextMeshProUGUI roundNumberText;
    [SerializeField] TextMeshProUGUI flavorTextMesh;
    [SerializeField] private CountDownDisplayManager manager;

    [SynchronizableField] public int time;
    [SynchronizableField] public static Color countdownColor = Color.green;
    private float deltaTime=0;
    public int maxTime;



    public static int sendTimeToUI;
    public static string sendFlavorTextToUI;
    public static int sendRoundsLeft;

    EndGameResolution endGameResolution;
    // countdowns left

    public static void ResetStatic()
    {
        countdownColor = Color.green;
        sendTimeToUI = 0;
        sendFlavorTextToUI = "";
    }

    private void Awake()
    {
        maxTime = time;
        sendRoundsLeft = manager.RoundsLeft;
    }

    private void Start()
    {
        sendFlavorTextToUI = flavorTextMesh.text;
        roundNumberText.text = manager.RoundsLeft.ToString();
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
            player.InitiateVotingPhase();
        }
    }
    [SynchronizableMethod]
    private void EndVotingPhaseForAllPlayers()
    {
        sendRoundsLeft = manager.RoundsLeft;
        roundNumberText.text = manager.RoundsLeft.ToString();
        sendRoundsLeft = manager.RoundsLeft;

        if(manager.RoundsLeft<=1)
        {
            flavorTextMesh.text = manager.lastChanceText;
            sendFlavorTextToUI = manager.lastChanceText;
        }


        //Debug.Log("vvv " + manager.RoundsLeft);

        VotingPhase player = Multiplayer.GetAvatar().gameObject.GetComponent<VotingPhase>();
        player.EndVotingPhase();
    }

    private void Update() {
        if (CountDownDisplayManager.hasInitiatedTheTimer)
        {
            if(!EndGameResolution.hasGameEnded)
            {
                UpdateTickDown();
                UpdateUI();
                sendTimeToUI = time;
            }
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
                    if(VotingPhase.totalALivePlayers.Count>1)
                    {
                        if(endGameResolution==null) endGameResolution = Multiplayer.GetAvatar().GetComponentInChildren<EndGameResolution>();
                        //if (!endGameResolution.inWildWest) {
                            time--;
                        //}
                    }
                }
            }
        }
        else
        {
            if (gameObject.CompareTag("VotingDisplay"))
            {
                manager.RoundsLeft--;
            }

            manager.BroadcastRemoteMethod("ActivateTimer", parameters: gameObject.name);
            BroadcastRemoteMethod(nameof(DeactivateUnusedTimers));
            
            if(gameObject.CompareTag("DowntimeDisplay")) BroadcastRemoteMethod(nameof(InitiateVotingPhaseForAllPlayers));
            if (gameObject.CompareTag("VotingDisplay")) BroadcastRemoteMethod(nameof(EndVotingPhaseForAllPlayers));
        }
    }
}
