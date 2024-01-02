using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/*
 * This class is used to generate UI for net work for game objects
 * Three buttons are used to start server, client and host
 */

public class NGO_UI : MonoBehaviour
{
    [SerializeField] private Button server;
    [SerializeField] private Button client;
    [SerializeField] private Button host;
    private void Awake()
    {
        server.onClick.AddListener(() => {NetworkManager.Singleton.StartServer();});
        client.onClick.AddListener(() => {NetworkManager.Singleton.StartClient();});
        host.onClick.AddListener(() => {NetworkManager.Singleton.StartHost();});
    }
}
