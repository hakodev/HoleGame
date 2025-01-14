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

        playerNumber++;
    }
    private void Start()
    {
        totalPlayers.Add(transform.root.GetComponent<PlayerRole>());
        foreach (PlayerRole role in totalPlayers)
        {
            Debug.Log(role.gameObject.name);
        }

        if (playerNumber != 1)
        {
        //    transform.parent.parent.gameObject.SetActive(false);
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
                Debug.Log("infils " + maxNumOfInfiltrators);
            }
        }
    }
    private void AssignRoles()
    {

        //Debug.Log($"Total players: {players.Count}");

        int randomNum;

        for (int i = 0; i < maxNumOfInfiltrators; i++)
        { // Give maxNumOfInfiltrators amount of random players the infiltrator role

            if (totalPlayers.Count == 0) break; // Just in case

            randomNum = Random.Range(0, totalPlayers.Count);

             totalPlayers[randomNum].BroadcastRemoteMethod("SetRole", Roles.Infiltrator);
            totalPlayers[randomNum].BroadcastRemoteMethod("DisplayRole");

            //            totalPlayers[randomNum].DisplayRole();

            // totalPlayers[randomNum].SetRole(Roles.Machine);
            // totalPlayers[randomNum].DisplayRole();
            Debug.Log(totalPlayers[randomNum].gameObject.name + totalPlayers[randomNum].GetRole());
            totalPlayers.RemoveAt(randomNum); // Remove the player from the roleless list after giving them a role
        }

        foreach(PlayerRole player in totalPlayers)
        { // Give the rest the machine role
            player.BroadcastRemoteMethod("SetRole", Roles.Machine);
            player.BroadcastRemoteMethod("DisplayRole");

           // player.SetRole(Roles.Machine);
           // player.DisplayRole();
            Debug.Log(player.gameObject.name + player.GetRole());

        }

        totalPlayers.Clear();

        //call all players
        gameObject.SetActive(false);
    }
}
public enum Roles
{
    Machine,
    Infiltrator
}
