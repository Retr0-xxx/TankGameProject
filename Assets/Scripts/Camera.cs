using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/*
 * script for camera follow
 * camera follow is done on the client side
 */
public class Camera : MonoBehaviour
{
    public Vector3 offset;
    public Vector3 rotation;
    private Transform target;
    //target is where the camera should look at
    //it should be set when the camera is instantiated
    public void setTarget(Transform target)
    {
        this.target = target;
    }

    //follow the target and look at it
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.fixedDeltaTime*5f);
        transform.rotation = Quaternion.Euler(rotation);
    }
}