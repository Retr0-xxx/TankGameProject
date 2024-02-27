
using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

//uses the unity lobby service to create a lobby for the game
public class LobbyLocal: MonoBehaviour
{
    private Lobby currLobby;
    private double TimeX = 0.0f;
    private string playerName;

    public static LobbyLocal Instance { get; private set; }


    private async void Start(){
        Instance = this;
        DontDestroyOnLoad(gameObject);

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
    //create a relay allocaton
    //returns the allocation 
    private async Task<Allocation> allocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(7); //MAX player count 8, can change it later
            Debug.Log("Allocated relay with ID: " + allocation.AllocationId);
            return allocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log("Failed to allocate relay: " + e.Message);
        }
        return default;
    }

    //takes an allocation and returns the join code for the relay
    private async Task<string> getRelayJoinCode(Allocation allocation)
    {
        string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        return relayJoinCode;
    }

    //join a relay with a join code
    //retuns a join relay allocation
    private async Task<JoinAllocation> joinRelay(string joinCode)
    {
        try
        { 
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            Debug.Log("Joined relay with join code: " + joinCode);
            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log("Failed to join relay: " + e.Message);
            return default;
        }
    }

    //creates a public lobby with specified name and maximum, store it as hostLobby
    public async void createLobby(string name, int maxPlayers)
    {
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = false;
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(name, maxPlayers,options); //create a public lobby of 8 people
            Allocation allocation = await allocateRelay(); //allocate a relay for the lobby
            string joinCode = await getRelayJoinCode(allocation); //get the join code for the relay
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation,"wss")); //set the relay server data with the allocation created and connection type (dtls encryption)
            await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions{ 
                Data = new Dictionary<string, DataObject>{
                    { "joinCode", new DataObject(DataObject.VisibilityOptions.Member,joinCode) } //key for join code is "joinCode"
                }
            }); //update the lobby data with the join code
            NetworkManager.Singleton.StartHost(); //start the host through the relay just specified
            NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single); //load the game scene
            currLobby = lobby;
            Debug.Log("Created lobby with name:"+name+" max player:"+maxPlayers+" ID:"+lobby.Id);
            
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
                Debug.Log("lobby name: "+lobby.Name+" max players: "+lobby.MaxPlayers+" id: "+lobby.Id+" current players: "+lobby.Players.Count+" isPrivate: "+lobby.IsPrivate);
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
            Lobby lobby =  await LobbyService.Instance.QuickJoinLobbyAsync(); //quick join a lobby
            string joinCode = lobby.Data["joinCode"].Value; //get the join code from the lobby data
            JoinAllocation joinAllocation = await joinRelay(joinCode); //join the relay with the join code
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "wss")); //set the relay server data with the join allocation and connection type
            NetworkManager.Singleton.StartClient();
            currLobby = lobby;

        }
        catch (RequestFailedException e)
        {
            Debug.Log("Failed to join lobby: " + e.Message);
        }
    }
    [Command]
    public async void JoinLobbyByID(string lobbyID)
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync("lobbyID");
            string joinCode = lobby.Data["joinCode"].Value; //get the join code from the lobby data
            JoinAllocation joinAllocation = await joinRelay(joinCode); //join the relay with the join code
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "wss")); //set the relay server data with the join allocation and connection type
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
