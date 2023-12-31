using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private float speed = 20f;

    // fly the bullet on server side
    // use sphrereCast to check if the bullet hits something

    // if it hits the player, tell the player to take damage
    // if it hits the block, tell the block to take damage
    // important: use fixed deltaTime to move the bullet
    void FixedUpdate()
    {       
        if(IsServer)
        {
           doBulletThing();
        }
    }
   
    void doBulletThing() 
    {
        float timeStep = Time.fixedDeltaTime;
        transform.Translate(Vector3.forward * speed * timeStep);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            try
            {
               other.GetComponent<PlayerHealth>().TakeDMG();
            }
            catch (System.Exception)
            {
                Debug.Log("Bullet missed player");
            }
            try
            {
                other.GetComponent<DesBlock>().TakeDMG();
            }
            catch (System.Exception)
            {
                Debug.Log("Bullet missed block");
            }
            NetworkObject.Despawn(gameObject);
        }
    }
    
}
