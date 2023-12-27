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

    //The server detects collision and tells the client to take damage
    private void OnTriggerEnter(Collider other)
    {
        if(!IsServer)
            return;

        if (other.CompareTag("Bullet")) 
        {
            TakeDMGClientRpc();
        }
    }

    //take damage on the client side
    [ClientRpc]
    private void TakeDMGClientRpc()
    {
        HP -= 30;
        Console.WriteLine("damage taken");
        if (HP <= 0)
        {
            Console.WriteLine("You are dead");
            Destroy(gameObject);
        }
    }
}
