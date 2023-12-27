using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private float speed = 20f;

    // the bullet is moving forward, handled by the server
    // use sphrereCast to check if the bullet hits something

    // if it hits the player, tell the player to take damage
    // if it hits the block, tell the block to take damage
    void Update()
    {
        if(!IsServer)
            return;

        transform.Translate(Vector3.forward*speed*Time.deltaTime);  
        if (Physics.SphereCast(transform.position,0.5f,transform.forward, out RaycastHit hit,speed*Time.deltaTime))
        { 
            try
            {
                hit.collider.GetComponent<PlayerHealth>().TakeDMG();
            }
            catch (System.Exception)
            {
                Debug.Log("Bullet missed player");
            }
            try
            {
                hit.collider.GetComponent<DesBlock>().TakeDMG();
            }
            catch (System.Exception)
            {
                Debug.Log("Bullet missed block");
            }
           NetworkObject.Despawn(gameObject);
        }
    }

    void setSpeed(float newSpeed) 
    {
        speed = newSpeed;
    }
}
