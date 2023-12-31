using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Collections;
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
    public GameObject CameraPrefab;
    public GameObject healthBarPrefab;
    private PlayerHealth playerHealth;
    private HealthBar healthBar;
    private Dictionary<PlayerHealth,HealthBar> playerHealthMap;

    void FixedUpdate()
    {
        //implement server authority for player movement
        //if it's the client, request the server to move the player
        if (IsLocalPlayer && IsClient)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            MoveServerRpc(horizontal, vertical);

            UpdateHealth();
        }

    }
    /*
     * This function is used to move the player
     * Do a box cast to check if the player collides with other objects
     * Movement is done on the server side
     * @param horizontal: the horizontal axis of the player
     * @param vertical: the vertical axis of the player
     */
    private void Start()
    {
      
        if (IsLocalPlayer && IsClient) 
        {
            //Instantiate a camera for the player
            //The camera is only visible to the local player, not shared on the network
            GameObject kamera = Instantiate(CameraPrefab, transform.position + Vector3.up * 10, Quaternion.identity);
            kamera.GetComponent<Camera>().setTarget(transform);

            //get a list of all the players in the game
            List<GameObject> players = GameObject.FindGameObjectsWithTag("Player").ToList();
            Debug.Log(players.Count);
            //Instantiate a health bar for all players
            //get both scripts for heath bar and player health
            //store them in a dictionary
            //exhange information of HP between them during update
            playerHealthMap = new Dictionary<PlayerHealth, HealthBar> { };
            foreach (GameObject player in players)
            {
                GameObject healthBarObj = Instantiate(healthBarPrefab,player.transform);
                playerHealth = player.GetComponent<PlayerHealth>();
                healthBar = healthBarObj.GetComponent<HealthBar>();
                int HP = playerHealth.getHP();
                healthBar.setHP(HP);
                healthBar.setCam(kamera.transform);
                playerHealthMap.Add(playerHealth,healthBar);
            }
        }
    }

    private void UpdateHealth()
    { 
      foreach (KeyValuePair<PlayerHealth,HealthBar> entry in playerHealthMap)
      {
          int HP = entry.Key.getHP(); 
          entry.Value.setHP(HP);
      }
    }

    private void Move(float horizontal,float vertical )
    {
        float timeStep = Time.fixedDeltaTime;
        Vector3 movement = transform.forward * timeStep * speed * vertical;
        bool canMove = !Physics.BoxCast(transform.position, transform.localScale, movement, Quaternion.identity, movement.magnitude);
        if (canMove)
        {
            if (vertical > 0f)
            {
                transform.Rotate(Vector3.up, horizontal*2);
                transform.Translate(movement, Space.World);
            }
            else if (vertical < 0f)
            {
                transform.Rotate(Vector3.up, -horizontal*2);
                transform.Translate(movement / 2, Space.World);
            }
            else
            {
                transform.Rotate(Vector3.up, horizontal*2);
            }
        }
    }        

    [ServerRpc]
    private void MoveServerRpc(float horizontal, float vertical) 
    {
        Move(horizontal, vertical);
    }

}
