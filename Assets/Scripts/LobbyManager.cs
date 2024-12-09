using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using System.Collections;

public class LobbyManager : MonoBehaviour
{
    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartBeatTimerMax = 15;
    private float heartBeatTimer = 15;
    private float lobbyUpdateTimer = 2f;
    private float lobbyUpdateTimerMax = 2f;
    private string playerName;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI lobbyText;
    [SerializeField] TextMeshProUGUI passwordText;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in" + AuthenticationService.Instance.PlayerId);
        };

        AuthenticationService.Instance.SignInAnonymouslyAsync();

        playerName = "player" + UnityEngine.Random.Range(10, 99);
    }

    private async void CreateLobby(string newLobbyName)
    {
        try
        {
            string lobbyName = newLobbyName;
            int maxPLayers = 12;
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    {  "GameMode", new DataObject(DataObject.VisibilityOptions.Public, "Normal") },
                    {  "Map", new DataObject(DataObject.VisibilityOptions.Public, "Office") }
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPLayers, createLobbyOptions);

            hostLobby = lobby;
            joinedLobby = lobby;

            Debug.Log("created lobby " + lobbyName + " with max players of " + maxPLayers);
            PrintPlayers(hostLobby);

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    private void Update()
    {
        LobbyHeartBeat();
        LobbyPollForUpdates();
    }
    private async void LobbyHeartBeat()
    {
        if (hostLobby != null)
        {
            heartBeatTimer -= Time.deltaTime;
            if (heartBeatTimer < 0)
            {
                heartBeatTimer = heartBeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }
    private async void LobbyPollForUpdates()
    {
        if (joinedLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0)
            {
                lobbyUpdateTimer = lobbyUpdateTimerMax;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
            }
        }
    }
    private async void JoinLobbyByCode(string code)
    {
        try
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(code, options);
            joinedLobby = lobby;
            Debug.Log("Joined lobby with code " + code);

            PrintPlayers(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                    }
        };
    }

    private async void UpdatePlayerName(string newPlayerName)
    {
        StartCoroutine(WaitForTime());

        if (joinedLobby != null)
        {
            try
            {
                playerName = newPlayerName;
                await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject> {
                {"playerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
                });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    private IEnumerator WaitForTime()
    {
        yield return new WaitForSeconds(1f);
        if(joinedLobby == null)
        {
            StartCoroutine(WaitForTime());
        }
        else
        {
            UpdatePlayerName(nameText.text);
        }
    }

    public async void EnterRoomButtonPressed()
    {
        try
        {
            JoinOrCreateLobby(lobbyText.text);
            UpdatePlayerName(nameText.text);
            GetComponent<JoinUI>().ExitUI();
        }
        catch (LobbyServiceException e) { 
            Debug.Log(e);
        }
    }
    private async void JoinOrCreateLobby(string enteredLobbyName)
    {
        bool exists = false;
        QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
        foreach (Lobby lobby in queryResponse.Results)
        {
            if (lobby.Name == enteredLobbyName)
            {
                exists = true;
                break;
            }
        }
        if (exists)
        {
            JoinLobbyByCode(enteredLobbyName);
            NetworkManager.Singleton.StartClient();

        }
        else
        {
            CreateLobby(enteredLobbyName);
            NetworkManager.Singleton.StartHost();
        }

    }
    //stuff i wont need 
    // private async void MigrateLobbyHost()
    // UpdateLobbbyGameMode() {}


    //stuff we're gonna need later if we have a more complex lobby system
    private async void QuickJoinLobby()
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in lobby " + lobby.Name);
        foreach (var player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }
    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter> {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder> {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };


            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log("lobbies found " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void KickPlayer()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[1].Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

}

