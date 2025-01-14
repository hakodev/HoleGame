using Alteruna;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoleAssignment : AttributesSync
{


    private static List<PlayerRole> rolelessPlayers = new();
    private static List<PlayerRole> totalPlayers = new List<PlayerRole>();

    private int maxNumOfInfiltrators = 1;
    Alteruna.Avatar avatar;

    [SerializeField] List<Vector2> InfiltratorsToPlayers;
    //x - alll players
    //y - humans
    //all int numbers

    [SynchronizableField] public static int playerNumber=0;

    private void Awake()
    {
        avatar = transform.root.GetComponent<Alteruna.Avatar>();
        totalPlayers.Add(transform.root.GetComponent<PlayerRole>());
        foreach (PlayerRole role in totalPlayers)
        {
            Debug.Log(role.gameObject.name);
        }
        playerNumber++;
    }
    private void Start()
    {
        if (playerNumber != 1)
        {
            gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        if(avatar.IsOwner)
        {
            if (Input.GetKeyUp(KeyCode.G))
            {
                FindRolelessPlayers();
                DetermineMaxNumberOfInfiltrators();
                AssignRoles();
            }
        }
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
            }
        }
    }
    private void AssignRoles()
    {

        //Debug.Log($"Total players: {players.Count}");

        int randomNum;

        for (int i = 0; i < maxNumOfInfiltrators; i++)
        { // Give maxNumOfInfiltrators amount of random players the infiltrator role

            if (rolelessPlayers.Count == 0) break; // Just in case

            randomNum = Random.Range(0, rolelessPlayers.Count);

            rolelessPlayers[randomNum].SetRole(Roles.Infiltrator);
            rolelessPlayers[randomNum].DisplayRole();
            rolelessPlayers.RemoveAt(randomNum); // Remove the player from the roleless list after giving them a role
        }

        foreach (PlayerRole player in rolelessPlayers)
        { // Give the rest the machine role
            player.SetRole(Roles.Machine);
        }

        rolelessPlayers.Clear();

        //call all players
        gameObject.SetActive(false);
    }
}
public enum Roles
{
    Machine,
    Infiltrator
}
