using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    [Header("Info")] 
    public int id;
    [Header("Stats")] 
    public float moveSpeed;
    public float jumpForce;
    [Header("Components")] 
    public Rigidbody rig;

   public Player photonPlayer;

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        GameManager.Instance.players[id - 1] = this;
        
        //If not our then
        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            rig.isKinematic = true;
        }
    }
    
    
    
    private void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryJump();
        }
    }

    void Move()
    {
        //Inputs
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        //Calculate Direction To Move

        Vector3 dir = (transform.forward * z + transform.right * x) * moveSpeed;
        dir.y = rig.velocity.y;


        rig.velocity = dir;
    }

    void TryJump()
    {
        //Check for Ground
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, 1.5f))
        {
            rig.AddForce(Vector3.up*jumpForce, ForceMode.Impulse);
        }
    }

}
