using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby lobby;

    public GameObject searchRoomButton;
    public GameObject cancelButton;

    private void Awake()
    {
        lobby = this;  // Create the singleton, lives withing the main menu scene
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();  // Connects to Master photon server.
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("player has connected to the Photon master server");
        PhotonNetwork.AutomaticallySyncScene = true;
        searchRoomButton.SetActive(true); // Player is now connected to the servers, enables the battlebutton to allow join a game.
    }

    public void OnSearchRoomButtonClicked()
    {
        Debug.Log("Battle Button was clicked");
        searchRoomButton.SetActive(false);
        cancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Try to join a game but failed. There must be no open game available");
        CreateRoom();
    }


    void CreateRoom()
    {
        Debug.Log("trying to create a room");
        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 10 };
        PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Try to create a new room but failed. There must be alread a room with the same name");
        CreateRoom();
    }

    public void OnCancelButtonClicked()
    {
        Debug.Log("Cancel Button was clicked");
        cancelButton.SetActive(false);
        searchRoomButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }
}
