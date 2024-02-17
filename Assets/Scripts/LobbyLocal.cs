using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
//uses the unity lobby service to create a lobby for the game
public class LobbyLocal: MonoBehaviour
{
    private Lobby currLobby;
    private double TimeX = 0.0f;
    private string playerName;

    public static LobbyLocal Instance { get; private set; }

    private async void Start(){
        Instance = this;

        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in with id:"+AuthenticationService.Instance.PlayerId);
        };

        //sign in anonymously, can later change to sign in with google or whatever
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        playerName = "Player" + UnityEngine.Random.Range(10, 99); //set default player name
    }

    void Update()
    {
       sendHeartBeat();
    }

    private async void sendHeartBeat()
    {
        if(currLobby != null)
        {
            TimeX += Time.deltaTime;
            if (TimeX > 15.0f)
            {
                TimeX = 0.0f;
                await LobbyService.Instance.SendHeartbeatPingAsync(currLobby.Id);
            }
        }
    }

    //creates a public lobby with specified name and maximum, store it as hostLobby
    public async void createLobby(string name, int maxPlayers)
    {
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = false;
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(name, maxPlayers,options);
            currLobby = lobby;
            Debug.Log("Created lobby with name:"+name+"max player:"+maxPlayers);
        }
        catch (RequestFailedException e)
        {
            Debug.Log("Failed to create lobby: " + e.Message);
        }
    }
    //give a list of all public lobbies
    [Command]
    public async void listLobbies()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            Debug.Log("Found " + queryResponse.Results.Count + " lobbies");
            foreach (var lobby in queryResponse.Results)
            {
                Debug.Log("lobby name: "+lobby.Name+" max players: "+lobby.MaxPlayers+" id:"+lobby.Id);
            }
        }
        catch (RequestFailedException e)
        {
            Debug.Log("Failed to list lobbies: " + e.Message);
        }
    }

    public async void quickJoinLobby()
    {
        try 
        {
            Lobby lobby =  await LobbyService.Instance.QuickJoinLobbyAsync();
            NetworkManager.Singleton.StartClient();
            currLobby = lobby;

        }
        catch (RequestFailedException e)
        {
            Debug.Log("Failed to join lobby: " + e.Message);
        }
    }

    private void setPlayerName(string playerName) 
    {
        this.playerName = playerName+UnityEngine.Random.Range(10,99);
    }

}
