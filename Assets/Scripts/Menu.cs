using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class Menu : MonoBehaviourPunCallbacks, ILobbyCallbacks
{

    [Header("Screens")] 
     public GameObject mainScreen;
     public GameObject createRoomScreen;
     public GameObject lobbyScreen;
     public GameObject lobbyBrowserScreen;

     [Header("MainScreen")] 
     public Button createRoomButton;
     public Button findRoomButton;

     [Header("Lobby")]
     public TextMeshProUGUI playerListText;

     public TextMeshProUGUI roomInfoText;
     public Button startGameButton;

     [Header("Lobby Browser")] 
     public RectTransform roomListContainer;

     public GameObject roomButtonPrefab;
     private List<GameObject> roomButtons = new List<GameObject>();
     private List<RoomInfo> roomList = new List<RoomInfo>();

     private void Start()
     {
         //Disable the button at Start
         createRoomButton.interactable = false;
         findRoomButton.interactable = false;
         
         //Enable the cursor
         Cursor.lockState = CursorLockMode.None;
         
         // Are we In a Game??
         if (PhotonNetwork.InRoom)
         {
             // Go to The lobby
             
             // Make the room Visible

             PhotonNetwork.CurrentRoom.IsVisible = true;
             PhotonNetwork.CurrentRoom.IsOpen = true;
         }
     }
     
     //<summary>Disables All The Screen and set the Screen to the Requested One<summary/>
     void SetScreen(GameObject screen)
     {
         //Disable Others
         mainScreen.SetActive(false);
         lobbyScreen.SetActive(false);
         lobbyBrowserScreen.SetActive(false);
         createRoomScreen.SetActive(false);
         //Enable Only This
         screen.SetActive(true);

         if (screen==lobbyBrowserScreen)
         {
             UpdateLobbyBrowserUI();
         }
     }

     public void OnBackButton()
     {
         SetScreen(mainScreen);
     }
     
     
     //Main Screen 
     public void OnPlayerNameValueChanged(TMP_InputField playerName)
     {
         PhotonNetwork.NickName = playerName.text;
     }

     public override void OnConnectedToMaster()
     {
         // Enable MainScreen Buttons
         createRoomButton.interactable = true;
         findRoomButton.interactable = true;
     }

     public void OnCreateRoomButton()
     {
         
         SetScreen(createRoomScreen);
     }

     public void OnFindRoomButton()
     {
         SetScreen(lobbyBrowserScreen);
     }
     
     // Create Room Screen

     public void OnCreateButton(TMP_InputField roomNameInputField)
     {
         
             NetworkManager.Instance.CreateRoom(roomNameInputField.text);
         
     }
     
     
     // Lobby Screen

     public override void OnCreatedRoom()
     {
         SetScreen(lobbyScreen);
         photonView.RPC("UpdateLobbyUI", RpcTarget.All);
     }

     public override void OnPlayerLeftRoom(Player otherPlayer)
     {
         UpdateLobbyUI();
     }

     [PunRPC]
     public void UpdateLobbyUI()
     {
         //Enable the start game button if master client
         startGameButton.interactable = PhotonNetwork.IsMasterClient;
         //Display All Players

         playerListText.text = "";

         foreach (var player in PhotonNetwork.PlayerList)
         {
             playerListText.text += player.NickName + "\n";
         }

         roomInfoText.text = "<b> Room Name </b> \n"+ PhotonNetwork.CurrentRoom.Name;
     }

     public void OnStartGameButton()
     {
         //hide the room
         PhotonNetwork.CurrentRoom.IsOpen = false;
         PhotonNetwork.CurrentRoom.IsVisible = false;
         
         NetworkManager.Instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
     }

     public void OnLeaveLobbyButton()
     {
         PhotonNetwork.LeaveRoom();
         SetScreen(mainScreen);
     }
     
     //LOBBY Browser Screen

     public override void OnJoinedRoom()
     {
         SetScreen(lobbyScreen);
         photonView.RPC("UpdateLobbyUI", RpcTarget.All);
     }

     GameObject CreateRoomButton()
     {
         GameObject buttonObj = Instantiate(roomButtonPrefab, roomListContainer.transform);
         roomButtons.Add(buttonObj);
         return buttonObj;
     }
     
     void UpdateLobbyBrowserUI()
     {
         //Disable all the buttons
         foreach (var button in roomButtons)  
         {
             button.SetActive(false);
         }
         
         //Display All Current Rooms in the MasterServer
         for (int x = 0; x < roomList.Count; ++x)
         {
             // Get Or Create?
             GameObject button = x >= roomButtons.Count ? CreateRoomButton() : roomButtons[x];
             button.SetActive(true);
            
             //Set the info about the button
             button.transform.Find("RoomNameText").GetComponent<TextMeshProUGUI>().text = roomList[x].Name;
             button.transform.Find("PlayerCountText").GetComponent<TextMeshProUGUI>().text =
                 roomList[x].PlayerCount + " / "+roomList[x].MaxPlayers;
             
             //Set the OnClickEvent

             Button buttonComp = button.GetComponent<Button>();
             string roomName = roomList[x].Name;
             buttonComp.onClick.RemoveAllListeners();
             buttonComp.onClick.AddListener(()=>{OnJoinRoomButton(roomName);});
         }
     }

  
     public void OnJoinRoomButton(string roomName)
     {
        
             
         NetworkManager.Instance.JoinRoom(roomName);
       
     }

     public void OnRefreshButton()
     {
         UpdateLobbyBrowserUI();
     }

     public override void OnRoomListUpdate(List<RoomInfo> allRooms)
     {
         roomList = allRooms;
     }

    
}
