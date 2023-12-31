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
    //player only takes damage on the server side

    //getHP is called from the client side
    //logic here: get HP calls hpServerRpc, which calls hpClientRpc
    //            passing the HP value on the server side to the client side
    public int getHP()
    {
         hpServerRpc();
         return HP;
    }
    [ServerRpc(RequireOwnership =false)]
    private void hpServerRpc() { hpClientRpc(HP); }

    [ClientRpc]
    private void hpClientRpc(int HP) { this.HP = HP;}


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
