using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InputManager : NetworkBehaviour
{
   public static InputManager Instance { get; set; }
   public NetworkVariable<bool> isControlDisabled = new NetworkVariable<bool>(false);

   void Awake()
    {
        Instance = this;
    }
   
}
