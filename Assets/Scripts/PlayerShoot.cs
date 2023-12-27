using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
/*
 * This script handles all the shooting behaviours
 * again, shooting is server authoritative
 */
public class PlayerShoot : NetworkBehaviour
{
    public GameObject bulletPrefab; 
   void Update()
    {
       if (IsLocalPlayer)
        {
            //if the player clicks the left mouse button, tell server to shoot
           if (Input.GetMouseButtonDown(0))
            {
               ShootServerRpc();
           }
       }
   }

    [ServerRpc]
    private void ShootServerRpc()
    {
        Shoot();
    }
    /*
     * This function instantiates a bullet prefab and spawns it on the server
     * The bullet will move itself forward automatically
     */
    private void Shoot()
    {
       GameObject bullet = Instantiate(bulletPrefab, transform.position + transform.forward, transform.rotation);
       bullet.GetComponent<NetworkObject>().Spawn();
    }
}
