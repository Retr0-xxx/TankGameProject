using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private float speed = 10f;
    void Update()
    {
        if(IsServer)
        transform.Translate(Vector3.forward*speed*Time.deltaTime);  
    }

    void setSpeed(float newSpeed) 
    {
        speed = newSpeed;
    }
}
