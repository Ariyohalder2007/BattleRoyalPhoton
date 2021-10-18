using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
   public int maxPlayers=10;
   
   //Instance 
   public static NetworkManager Instance { get;  private set;}


   private void Awake()
   {
      Instance = this;
      DontDestroyOnLoad(gameObject);
   }

   private void Start()
   {
      //Connect To Master Server
      PhotonNetwork.ConnectUsingSettings();
      
   }

   private void Update()
   {
      // Debug.Log(PhotonNetwork.IsConnected);
   }

   public override void OnConnectedToMaster()
   {
      PhotonNetwork.JoinLobby();
   }

   public override void OnJoinedLobby()
   {
      Debug.Log("Joined lobby");
      // PhotonNetwork.CreateRoom("test");
   }

   public override void OnCreatedRoom()
   {
      Debug.Log(PhotonNetwork.CurrentRoom.Name);
   }

   public void CreateRoom(string roomName)
   {

      RoomOptions roomOptions = new RoomOptions();
      roomOptions.MaxPlayers = (byte)maxPlayers;
      PhotonNetwork.CreateRoom(roomName, roomOptions);
      
   }

   public void JoinRoom(string roomName)
   {
      
      PhotonNetwork.JoinRoom(roomName);
   }
   [PunRPC]
   public void ChangeScene(string sceneName)
   {
      PhotonNetwork.LoadLevel(sceneName);
   }
}
