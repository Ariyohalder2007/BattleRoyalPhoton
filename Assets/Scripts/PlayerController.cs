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
    private int curAttackerId;
    [Header("Stats")] 
    public float moveSpeed;
    public float jumpForce;
    public int curHp;
    public int maxHp;
    public int kills;
    public bool dead;

    private bool flashingDamage;
    [Header("Components")] 
    public Rigidbody rig;

    public PlayerWeapon weapon;
    public MeshRenderer mr;
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
        else
        {
            GameUI.Instance.Initialize(this);
        }
    }
    
    
    
    private void Update()
    {
        if (!photonView.IsMine || dead)
        {
            return;
            
        }
        
        Move();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryJump();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            weapon.TryShoot();
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
    
    [PunRPC]
    public void TakeDamage(int attackerId, int damage)
    {
        if (dead) return;
        curHp -= damage;
        curAttackerId = attackerId;
        //Flash the player RED
        photonView.RPC("DamageFlash", RpcTarget.Others);
        
        
        //Update the player UI
        GameUI.Instance.UpdateHealthBar();
        
        
        //DIE if health == 0

        if (curHp<=0)
        {
            photonView.RPC("Die", RpcTarget.All);
        }
    }

    [PunRPC]
    public void Die()
    {
        curHp = 0;
        dead = true;
        GameManager.Instance.alivePlayers--;
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.CheckWinCondition();
        }

        if (photonView.IsMine)
        {
            if (curAttackerId != 0)
            {
                GameManager.Instance.GetPlayer(curAttackerId).photonView.RPC("AddKill", RpcTarget.All);
            }
            
            //Set the Cam to Spectator View
            GetComponentInChildren<CameraController>().SetAsSpectator();
            //Disable the physics and hide the player
            rig.isKinematic = true;
            transform.position = new Vector3(0, -50, 0);
        }
    }

    [PunRPC]
    void AddKill()
    {
        kills++;
        // Update UI for kills
        GameUI.Instance.UpdatePlayerInfoText();
        
    }

    [PunRPC]
    public void DamageFlash()
    {
        if (flashingDamage) return;
        StartCoroutine(DamageFlashCoRoutine());


        IEnumerator DamageFlashCoRoutine()
        {
            flashingDamage = true;
            Color defColor = mr.material.color;
            mr.material.color=Color.red;
            yield return new WaitForSeconds(.5f);
            mr.material.color = defColor;
            flashingDamage = false;
        }
    }

    [PunRPC]
    public void Heal(int amount)
    {
        curHp = Mathf.Clamp(curHp + amount, 0, maxHp);
        // Update HealthBar
        GameUI.Instance.UpdateHealthBar();
        
        
    }

}
