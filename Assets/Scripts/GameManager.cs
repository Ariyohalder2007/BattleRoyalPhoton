using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourPun
{
   public float postGameTime;
   [Header("Player")] 
   public string playerPrefabLocation;

   public PlayerController[] players;
   public Transform[] spawnPoints;
   public int alivePlayers;
   private int playersInGame;
   
   //Instance;
   public static GameManager Instance { get; private set; }

   private void Awake()
   {
      Instance = this;
   }

   private void Start()
   {
      players = new PlayerController[PhotonNetwork.PlayerList.Length] ;
      alivePlayers = players.Length;
      photonView.RPC("ImInGame", RpcTarget.AllBuffered);
   }
   [PunRPC]
   void ImInGame()
   {
      playersInGame++;
      if (PhotonNetwork.IsMasterClient && playersInGame==PhotonNetwork.PlayerList.Length)
      {
         photonView.RPC("SpawnPlayer", RpcTarget.All);
      }
   }

   [PunRPC]
   void SpawnPlayer()
   {
      GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation,
         spawnPoints[Random.Range(0, spawnPoints.Length)].position, 
         Quaternion.identity);
      
      //Initialize the player
      
      playerObj.GetComponent<PlayerController>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
   }

   public PlayerController GetPlayer(int playerId)
   {
      return players.First(x => x.id == playerId);
   }

   public PlayerController GetPlayer(GameObject playerObject)
   {
      return players.First(x => x.gameObject == playerObject);
   }

   public void CheckWinCondition()
   {
      if (alivePlayers==1)
      {
         photonView.RPC("WinGame", RpcTarget.All, players.First(x=>!x.dead).id);
      }
   }

   [PunRPC]
   public void WinGame(int winningPlayer)
   {
      //Set the UI win Text
      
      
      Invoke("GoBackToMenu", postGameTime);
   }

   void GoBackToMenu()
   {
      NetworkManager.Instance.ChangeScene("Menu");
      
   }
}
