using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Stats")] 
    public int damage;
    public int curAmmo;
    public int maxAmmo;
    public float bulletSpeed;
    public float shootRate;

    private float lastShootTime;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPos;

    private PlayerController player;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    public void TryShoot()
    {
        //Can We shoot??
        if (curAmmo<=0|| Time.time - lastShootTime<shootRate) return;
        curAmmo--;

        lastShootTime = Time.time;
        
        //Update the ammo UI
        
        //Spawn the Bullet
        player.photonView.RPC("SpawnBullet", RpcTarget.All, bulletSpawnPos.position, Camera.main.transform.forward);
    }
    
    [PunRPC]
    public void SpawnBullet(Vector3 pos, Vector3 dir)
    {
        //SpawnBullet
        GameObject bulletObj = Instantiate(bulletPrefab, pos, Quaternion.identity);
        bulletObj.transform.forward = dir;

        if (bulletObj.TryGetComponent<Bullet>(out var bulletScript))
        {
            bulletScript.Initialize(damage, player.id, player.photonView.IsMine);
            bulletScript.rig.velocity = dir * bulletSpeed; 
        }
        

    }
}
