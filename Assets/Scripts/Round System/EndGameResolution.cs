
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Alteruna;


public class EndGameResolution : AttributesSync
{
    // alive players gotten from PlayerRole
    int infiltratorsCount = 0;
    int machinesCount = 0;

    CountDownDisplayManager display;
    Canvas endGameCanvas;
    TextMeshProUGUI descriptor;
    [SerializeField] PopUp popUp;
    [SerializeField] CanvasGroup machinesWon;
    [SerializeField] CanvasGroup infiltratorsWon;

    [SerializeField] string timerEndedText = "Timer Ended";
    [SerializeField] string allInfiltratorsDead = "No Infiltrators Left";
    [SerializeField] string allMachinesDead = "No Machines Left";


    [SerializeField] PopUp votingPopUp;
    [SerializeField] TextMeshProUGUI wildWestTaskBar;
    [SerializeField] TextMeshProUGUI wildWestTitle;
    [SerializeField] TextMeshProUGUI wildWestExplanatoryText;
    [SerializeField] string wildWestProtocolText = "Wild West Protocol";
    [SerializeField] string wildWestTitleText = "Fire The Other Worker";


    [SynchronizableField] public bool inWildWest = false;
    public static bool hasGameEnded = false;

    private void Awake()
    {
        endGameCanvas = GetComponent<Canvas>();
        popUp = transform.GetComponentInChildren<PopUp>();
    }
    private void Start()
    {

        display = FindAnyObjectByType<CountDownDisplayManager>();
        wildWestExplanatoryText.gameObject.SetActive(false);
        popUp.gameObject.SetActive(false);
        hasGameEnded = false;
    }

    public void CheckForEndGame()
    {
        RecountPlayers();

        if (VotingPhase.totalALivePlayers.Count==2 && infiltratorsCount == 1 && machinesCount ==1) WildWest();

        if (CountdownDisplay.sendRoundsLeft<=0) GroupWon(infiltratorsWon ,timerEndedText);
        if (infiltratorsCount == 0) GroupWon(machinesWon, allInfiltratorsDead);
        if (machinesCount == 0) GroupWon(infiltratorsWon, allMachinesDead);      
    }

    private void RecountPlayers()
    {
        infiltratorsCount = 0;
        machinesCount = 0;
        foreach(PlayerRole player in VotingPhase.totalALivePlayers)
        {
            if(player.GetRole() == Roles.Infiltrator) infiltratorsCount++;
            if(player.GetRole() == Roles.Machine) machinesCount++;
        }

        Debug.Log("infils " + infiltratorsCount + "machines " + machinesCount);
    }

    private void WildWest()
    {
        wildWestExplanatoryText.gameObject.SetActive(true);

        wildWestTaskBar.text = wildWestProtocolText;
        wildWestTitle.text = wildWestTitleText;

        inWildWest = true;

        votingPopUp.ToggleTriggerCaptcha(false);
    }
    public void HandOutGuns()
    {
       // foreach (PlayerRole player in VotingPhase.totalALivePlayers)
       // {
            transform.root.GetComponent<Interact>().SpecialInteraction(InteractionEnum.GivenTaskManagerRole, this);
      //  }
    }

    private bool once = true;
    private void Update()
    {
        if (inWildWest && once) {
            if(VotingPhase.totalALivePlayers.Count<=1)
            {
                once = false;
                CheckForEndGame();
            }
        }
    }

    private void GroupWon(CanvasGroup group, string description)
    {
        hasGameEnded = true;

        popUp.gameObject.SetActive(true);
        popUp.PopIn();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(group.DOFade(0f, 0.2f));
        sequence.Append(group.DOFade(1f, 1f));

        descriptor = group.transform.Find("Descriptor").GetComponent<TextMeshProUGUI>();
        descriptor.text = description;
    }

    public void ReloadScene()
    {
        RoleAssignment.ResetStatic();
        CountdownDisplay.ResetStatic();
        PlayerRole.ResetStatic();

        VotingPhase.StaticReset();

        //destroy the dontdestroyonloads now if you have any

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
