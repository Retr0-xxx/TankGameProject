using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
/*
 * This class is used to handle player health
 * Player can take damage from bullets
 * It's server authoritative
 */
public class PlayerHealth : NetworkBehaviour
{
    private int HP = 100;

    //this function gets called on the server side when the player gets hit by a bullet
    
    public void TakeDMG()
    {
        if (!IsServer)
            return;

        HP -= 20;
        Debug.Log("Player took damage");
        if (HP <= 0)
        {
            Debug.Log("Player died");
            NetworkObject.Despawn(gameObject);
        }
    }
}
