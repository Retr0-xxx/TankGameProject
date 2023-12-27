using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DesBlock : NetworkBehaviour
{
    private int HP = 100;

    //this function gets called on the server side when the object gets hit by a bullet
    public void TakeDMG()
    {
        if(!IsServer)
            return;

        HP -= 50;
        Debug.Log("Block took damage");
        if (HP <= 0)
        {
            Debug.Log("Block Destroyed");
            NetworkObject.Despawn(gameObject);
        }
    }
}
