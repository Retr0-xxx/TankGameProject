using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Netcode;
using UnityEditor.Rendering;
using UnityEngine;
/*
 * This class is used to handle player movement
 * The player is a tank that can be controlled by WASD
 */
public class PlayerController : NetworkBehaviour
{
    public float speed = 1f;
  
    void Update()
    {
        //implement server authority for player movement
        //if it's the client, request the server to move the player
        if (IsLocalPlayer)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            MoveServerRpc(horizontal, vertical);
        }

    }
    /*
     * This function is used to move the player
     * Do a box cast to check if the player collides with other objects
     * Movement is done on the server side
     * @param horizontal: the horizontal axis of the player
     * @param vertical: the vertical axis of the player
     */
    private void Move(float horizontal,float vertical )
    {
        Vector3 movement = transform.forward * Time.fixedDeltaTime * speed * vertical / 10f;
        bool canMove = !Physics.BoxCast(transform.position, transform.localScale, movement, Quaternion.identity, movement.magnitude);
        if (canMove)
        {
            if (vertical > 0f)
            {
                transform.Rotate(Vector3.up, horizontal / 2);
                transform.Translate(movement, Space.World);
            }
            else if (vertical < 0f)
            {
                transform.Rotate(Vector3.up, -horizontal / 2);
                transform.Translate(movement / 2, Space.World);
            }
            else
            {
                transform.Rotate(Vector3.up, horizontal/2);
            }
        }
    }        

    [ServerRpc]
    private void MoveServerRpc(float horizontal, float vertical) 
    {
        Move(horizontal, vertical);
    }

}
