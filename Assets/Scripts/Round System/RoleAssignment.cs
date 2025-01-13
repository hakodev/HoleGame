using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleAssignment : MonoBehaviour
{


    private List<PlayerRole> rolelessPlayers = new();
    private int maxNumOfInfiltrators = 1;

    [SerializeField] List<InfiltratorsToPlayers> infiltratorsToPlayers = new List<InfiltratorsToPlayers>();

    private void OnEnable()
    {
        FindRolelessPlayers();
        DetermineMaxNumberOfInfiltrators();
        AssignRoles();
    }

    private void FindRolelessPlayers()
    {
        PlayerRole[] totalPlayers = FindObjectsByType<PlayerRole>(FindObjectsSortMode.None);
        rolelessPlayers.AddRange(totalPlayers);
    }
    private void DetermineMaxNumberOfInfiltrators()
    {
        foreach (InfiltratorsToPlayers ratio in infiltratorsToPlayers)
        {
            if (rolelessPlayers.Count >= ratio.robotsCount)
            {
                maxNumOfInfiltrators = ratio.infiltratorsCount;
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
            rolelessPlayers.RemoveAt(randomNum); // Remove the player from the roleless list after giving them a role
        }

        foreach (PlayerRole player in rolelessPlayers)
        { // Give the rest the machine role
            player.SetRole(Roles.Machine);
        }

        rolelessPlayers.Clear();

        //call all players
    }


}
public class InfiltratorsToPlayers
{
    public int infiltratorsCount;
    public int robotsCount;

    public InfiltratorsToPlayers(int infiltratorsCount, int robotsCount)
    {
        this.infiltratorsCount = infiltratorsCount;
        this.robotsCount = robotsCount;
    }
}
public enum Roles
{
    Machine,
    Infiltrator
}
