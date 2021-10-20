using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
   public Slider healthBar;
   public TextMeshProUGUI playerInfoText;
   public TextMeshProUGUI ammoText;
   public TextMeshProUGUI winnerText;
   public Image winBkg;
   private PlayerController player;

   public static GameUI Instance{ get; private set; }

   private void Awake()
   {
      Instance = this;
   }

   public void Initialize(PlayerController localPlayer)
   {
      player = localPlayer;
      healthBar.maxValue = player.maxHp;
      healthBar.value = player.curHp;
      
      UpdatePlayerInfoText();
      UpdateAmmoText();
      
      
   }

   public void UpdateHealthBar()
   {
       healthBar.value = player.curHp;
      
   }

   public void UpdatePlayerInfoText()
   {
      playerInfoText.text = "<b> Alive: </b>" + GameManager.Instance.alivePlayers
         +"\n <b> Kills: </b>"+player.kills;
   }

   public void UpdateAmmoText()
   {
      ammoText.text = player.weapon.curAmmo + " / " + player.weapon.maxAmmo;
   }

   public void SetWinText(string winnerName)
   {
      winnerText.SetText(winnerName+" Wins! ");
      winBkg.gameObject.SetActive(true);
      
   }
}
