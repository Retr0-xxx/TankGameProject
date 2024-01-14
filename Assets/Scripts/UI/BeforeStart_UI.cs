using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
using Unity.VisualScripting;
//name is a bit misleading, this controls two UI elements, beforeStart_UI and waiting_UI
public class BeforeStart_UI : MonoBehaviour
{
    public GameObject beforeStart_UI;
    public GameObject waiting_UI;
    public GameObject countDown_UI;
    void Start()
    {
        StateManager.Instance.onLocalPlayerReadyChanged += onReady_UI;
        StateManager.Instance.onCountDownStart += onCountDownStart_UI;
        StateManager.Instance.onInGame += onInGame_UI;
    }

    //when the local player is ready event fired, show the waiting UI
    void onReady_UI(object sender, EventArgs e) 
    {
       if(StateManager.Instance.isLocalPlayerReady)
       { 
           beforeStart_UI.SetActive(false);
            waiting_UI.SetActive(true);
       }
       else
       {
          beforeStart_UI.SetActive(true);
            waiting_UI.SetActive(false);
       }
    }
    //when the countdown starts event fired, hide the waiting UI and show the countdown UI
    void onCountDownStart_UI(object sender, EventArgs e)
    {
        beforeStart_UI.SetActive(false);
        waiting_UI.SetActive(false);
        countDown_UI.SetActive(true);
    }

    //when the game starts event fired, hide all UI
    void onInGame_UI(object sender, EventArgs e)
    {
        beforeStart_UI.SetActive(false);
        waiting_UI.SetActive(false);
        countDown_UI.SetActive(false);
    }
}
