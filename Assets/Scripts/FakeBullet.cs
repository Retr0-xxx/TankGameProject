using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeBullet : MonoBehaviour
{
    private float speed = 20f;
    void FixedUpdate()
    {
       doBulletThing();
    }

    void doBulletThing()
    {
        float timeStep = Time.fixedDeltaTime;
        transform.Translate(Vector3.forward * speed * timeStep);
    }

    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
