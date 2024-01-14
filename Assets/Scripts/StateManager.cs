using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StateManager : NetworkBehaviour
{  public static StateManager Instance { get; private set; }

    private Dictionary<ulong, bool> ReadyClientDic;
    public bool isLocalPlayerReady;
    //event handlers for StateManager
    public event EventHandler onStateChange;
    public event EventHandler onCountDownStart;
    public event EventHandler onInGame;
    public event EventHandler onLocalPlayerReadyChanged;
    //any modification to the state will automatically fire a onStateChange event
    public NetworkVariable<State> state = new NetworkVariable<State>(State.waitStart);

    void Awake()
    {
        Instance = this;
        isLocalPlayerReady = false;
        ReadyClientDic = new Dictionary<ulong, bool>();
        onStateChange += onStageChangeMethod;
        onCountDownStart += onCountStartMethod;

    }
    
    //fire a stage change event the moment the state changes
    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += (previousValue, newValue) => onStateChange?.Invoke(this, EventArgs.Empty);
        //invoke the event once to disable player control
        onStateChange?.Invoke(this, EventArgs.Empty);
    }

    public enum State
    {
        waitStart,
        countDownStart,
        inGame,
        gameOver
    }


    
    //method is called each time a player press space
    //on the serverside, store the true ready state of all players in a dictionary
    //loop through the dictionary, if all players are ready, start the countdown
    [ServerRpc(RequireOwnership = false)]
    public void ClientReadyServerRpc(bool value ,ServerRpcParams serverRpcParams=default)
    {
        ulong id = serverRpcParams.Receive.SenderClientId;
        ReadyClientDic[id] = value;
        bool allReady = true;
        int readyCount = 0;
        int notReadyCount = 0;
        foreach (ulong clientIDs in NetworkManager.Singleton.ConnectedClientsIds) 
        {
            if (!ReadyClientDic[clientIDs] || !ReadyClientDic.ContainsKey(clientIDs))
            {
                allReady = false;
                notReadyCount++;
            }
            else
            {
                readyCount++;
            }
        }
        if (allReady && readyCount!=1)
        {
            state.Value = State.countDownStart;
            Debug.Log("All Players Ready, Starting Count Down...");
        }
        else
        {
            Debug.Log("you need at least two players to start the game!");
        }
        Debug.Log("Player with clientID " + id + "is ready");
        Debug.Log(readyCount+" Players Ready //"+notReadyCount+" Players Not Ready");
    }

    //listening to onStateChange event
    //according to the state, events are fired
    private void onStageChangeMethod(object sender, EventArgs e)
    {
        switch (state.Value)
        {
            case State.waitStart:
                InputManager.Instance.isControlDisabled.Value = true;
                break;
            case State.countDownStart:
                onCountDownStart?.Invoke(this, EventArgs.Empty);
                InputManager.Instance.isControlDisabled.Value = true;
                break;
            case State.inGame:
                onInGame?.Invoke(this, EventArgs.Empty);
                InputManager.Instance.isControlDisabled.Value = false;
                break;
            case State.gameOver:
                InputManager.Instance.isControlDisabled.Value = true;
                break;
            default:
                break;
        }
    }

    //if player press space, toggle ready, and send to server, fired the player ready event
    private void checkPlayerReady() 
    {
        
        if (Input.GetKeyDown(KeyCode.Space) && state.Value.Equals(State.waitStart))
        {
            Debug.Log("Space Pressed");
            if (isLocalPlayerReady)
            {
                isLocalPlayerReady = false;
                ClientReadyServerRpc(false);
            }
            else
            {
                isLocalPlayerReady = true;
                ClientReadyServerRpc(true);
            }
            onLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    //listening to the onCountDownStart event, start a countdown coroutine
    private void onCountStartMethod(object sender, EventArgs e)
    {
        StartCoroutine(countDown());
    }
    

    private void Update()
    {
        checkPlayerReady();
    }

    //countdown coroutine, change the state to inGame when countdown finishes
    IEnumerator countDown()
    {
        int count = 3;
        while (count > 0)
        {
            Debug.Log(count);
            yield return new WaitForSeconds(1);
            count--;
        }
        state.Value = State.inGame;
    }


}
