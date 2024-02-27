using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button quickJoin;
    [SerializeField] private Button createLobby;

    void Start()
    {
        quickJoin.onClick.AddListener(() => { LobbyLocal.Instance.quickJoinLobby();});
        //will later make lobby name of max player customizable
        createLobby.onClick.AddListener(() => { LobbyLocal.Instance.createLobby("Lobby" + UnityEngine.Random.Range(10, 99), 8); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
