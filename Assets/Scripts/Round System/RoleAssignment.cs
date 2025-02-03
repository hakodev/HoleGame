using Alteruna;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoleAssignment : AttributesSync
{
    private List<PlayerRole> rolelessPlayers = new List<PlayerRole>();
    private static List<PlayerRole> totalPlayers = new List<PlayerRole>();
    public static bool hasGameStarted = false;

    private int maxNumOfInfiltrators = 1;
    Alteruna.Avatar avatar;
    public static Alteruna.Avatar userAvatar;
    CanvasGroup youNeedFriends;


    [SerializeField] List<Vector2> InfiltratorsToPlayers;
    //x - alll players
    //y - humans
    //all int numbers

    [SerializeField] private GameObject lobbyCanvas;


    [SynchronizableField] public static int playerNumber=0;
    public static int playerID=-10; //client based id
    [SynchronizableField] bool assignedRole = false;


    public static void ResetStatic()
    {
        hasGameStarted = false;
        playerID = -10;
        playerNumber = 0;
        totalPlayers.Clear();
        userAvatar = null;
    }
    private void Awake()
    {

        playerNumber++;
    }
    private void Start()
    {
        totalPlayers.Add(transform.root.GetComponent<PlayerRole>());
        avatar = transform.root.GetComponent<Alteruna.Avatar>();
        if(avatar.IsMe) userAvatar = avatar;
        youNeedFriends = transform.parent.Find("YouNeedFriendsToStartGame").GetComponent<CanvasGroup>();
        // totalPlayers = FindObjectsByType<PlayerRole>(FindObjectsSortMode.None).ToList();

        if (playerID == -10) playerID = playerNumber; //sets proper number
        Debug.Log("player's count " + totalPlayers.Count);

        if (playerNumber != 1)
        {
        //    transform.parent.parent.gameObject.SetActive(false);
        gameObject.SetActive(false);
            
        }
    }
    [SynchronizableMethod]
    public void SetHasGameStarted(bool newState)
    {
        hasGameStarted = newState;
    }


    private void Update()
    {
        if(!hasGameStarted)
        {
            if (Multiplayer.GetUser().IsHost)
            {
                if (Input.GetKeyUp(KeyCode.G))
                {
                    if (totalPlayers.Count > 1)
                    {
                        BroadcastRemoteMethod(nameof(SetHasGameStarted), true);
                        FindRolelessPlayers();
                        DetermineMaxNumberOfInfiltrators();
                        AssignRoles();

                        //BroadcastRemoteMethod(nameof(DestroyLobbyForAll));
                        SymptomsManager.Instance.PickRandNumberHostAndSetSymptomForAll();
                        //SymptomsManager.Instance.BroadcastRemoteMethod(nameof(SymptomsManager.Instance.SetSymptom), SymptomsManager.Instance.GetRandNumber());
                        VotingPhase voting = transform.root.GetComponent<VotingPhase>();
                        voting.AllVotersSymptomNotifStartOfGame();
                    }
                    else
                    {
                        StartCoroutine(DisplayFriends());
                    }
                }
            }
        }
    }
    /*
    public static List<PlayerRole> GetTotalPlayers() {
        return totalPlayers;
    }
    */



    [SynchronizableMethod]
    private void DestroyLobbyForAll()
    {
        //yeah so im not doing this
        Debug.Log("damn " + Multiplayer.GetUser());
        
    }

    private void FindRolelessPlayers()
    {
        rolelessPlayers.AddRange(totalPlayers);
    }
    private void DetermineMaxNumberOfInfiltrators()
    {
        foreach (Vector2 ratio in InfiltratorsToPlayers)
        {
            if (rolelessPlayers.Count >= ratio.x)
            {
                maxNumOfInfiltrators = (int)ratio.y;
                Debug.Log("infils " + maxNumOfInfiltrators);
            }
        }
    }
    private void AssignRoles()
    {
        int randomNum;

        for (int i = 0; i < maxNumOfInfiltrators; i++)
        { // Give maxNumOfInfiltrators amount of random players the infiltrator role

            if (rolelessPlayers.Count == 0) break; // Just in case

            randomNum = Random.Range(0, rolelessPlayers.Count);

            rolelessPlayers[randomNum].BroadcastRemoteMethod("SetRole", Roles.Infiltrator);
            rolelessPlayers[randomNum].BroadcastRemoteMethod("DisplayRole");

            rolelessPlayers.RemoveAt(randomNum); // Remove the player from the roleless list after giving them a role
        }

        foreach(PlayerRole player in rolelessPlayers)
        { // Give the rest the machine role
            player.BroadcastRemoteMethod("SetRole", Roles.Machine);
            player.BroadcastRemoteMethod("DisplayRole");
        }

        rolelessPlayers.Clear();

       GetComponent<TextMeshProUGUI>().enabled = false;
    }
    private IEnumerator DisplayFriends()
    {
        youNeedFriends.DOFade(1f, 1f);
        yield return new WaitForSeconds(2f); // How many seconds to display it on screen
        youNeedFriends.DOFade(0f, 1f);
    }

}
public enum Roles
{
    Machine,
    Infiltrator
}
