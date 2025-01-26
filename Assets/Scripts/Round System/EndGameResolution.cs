
using UnityEngine;
using TMPro;
using System.Collections;
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


    [SerializeField] TextMeshProUGUI wildWestTaskBar;
    [SerializeField] TextMeshProUGUI wildWestTitle;
    [SerializeField] TextMeshProUGUI wildWestExplanatoryText;
    [SerializeField] string wildWestProtocolText = "Wild West Protocol";
    [SerializeField] string wildWestTitleText = "Fire The Other Worker";


    [SynchronizableField] public bool inWildWest = false;


    private void Awake()
    {
        display = FindAnyObjectByType<CountDownDisplayManager>();
        endGameCanvas = GetComponent<Canvas>();
        popUp = transform.GetComponentInChildren<PopUp>();
    }
    private void Start()
    {
        wildWestExplanatoryText.gameObject.SetActive(false);
        popUp.gameObject.SetActive(false);
    }

    public void CheckForEndGame()
    {
        RecountPlayers();

        if (VotingPhase.totalALivePlayers.Count==2 && infiltratorsCount == 1 && machinesCount ==1) WildWest();

        if (display.TimeToEndTheGame<=0) GroupWon(infiltratorsWon ,timerEndedText);
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
    }

    private void WildWest()
    {
        wildWestExplanatoryText.gameObject.SetActive(true);

        wildWestTaskBar.text = wildWestProtocolText;
        wildWestTitle.text = wildWestTitleText;

        inWildWest = true;
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
        popUp.gameObject.SetActive(true);
        popUp.PopIn();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(machinesWon.DOFade(0f, 0.2f));
        sequence.Append(machinesWon.DOFade(1f, 1f));

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
