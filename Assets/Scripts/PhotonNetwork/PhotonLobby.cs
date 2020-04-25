using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class PhotonLobby : MonoBehaviourPunCallbacks {
	public static PhotonLobby lobby;

	public GameObject ConnectingIndicator;
	public GameObject RoomOperationArea;
	public GameObject cancelButton;
	public InputField RoomNameInput;

	public GameObject RoomListPanel;
	public GameObject RoomItemPrefab;

	private void Awake() {
		lobby = this;  // Create the singleton, lives withing the main menu scene
	}

	// Start is called before the first frame update
	void Start() {
		PhotonNetwork.ConnectUsingSettings();  // Connects to Master photon server.
	}

	public override void OnConnectedToMaster() {
		Debug.Log("player has connected to the Photon master server");
		PhotonNetwork.AutomaticallySyncScene = true;
		PhotonNetwork.JoinLobby();
	}
	public override void OnJoinedLobby() {
		base.OnJoinedLobby();
		ConnectingIndicator.SetActive(false);
		RoomOperationArea.SetActive(true); // Player is now connected to the servers, enables the battlebutton to allow join a game.
	}
	public override void OnCreateRoomFailed(short returnCode, string message) {
		Debug.Log("Try to create a new room but failed. There must be alread a room with the same name");
		CreateRoomWithRandomName();
	}
	public override void OnJoinRandomFailed(short returnCode, string message) {
		Debug.Log("Try to join a game but failed. There must be no open game available");
		CreateRoomWithRandomName();
	}
	public override void OnRoomListUpdate(List<RoomInfo> roomList) {
		base.OnRoomListUpdate(roomList);
		foreach (Transform t in RoomListPanel.transform) {
			Destroy(t.gameObject);
		}
		foreach (RoomInfo room in roomList) {
			if (room.IsVisible) {
				GameObject item = Instantiate(RoomItemPrefab);
				RoomItemDisplay display = item.GetComponent<RoomItemDisplay>();
				display.Name = room.Name;
				display.NumPlayers = room.PlayerCount;
				display.MaxNumPlayers = room.MaxPlayers;
				display.transform.parent = RoomListPanel.transform;
				item.GetComponent<Button>().onClick.AddListener(() => {
					PhotonNetwork.JoinRoom(room.Name);
				});
			}
		}
	}

	private void CreateRoom(string name) {
		Debug.Log("trying to create room " + name);
		RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 6 };
		PhotonNetwork.CreateRoom(name, roomOps);
	}
	private void CreateRoomWithRandomName() {
		CreateRoom("Room " + Random.Range(0, 10000).ToString());
	}

	public void OnCreateRoomButtonClicked() {
		if (RoomNameInput.text.Length > 0) {
			CreateRoom(RoomNameInput.text);
		} else {
			CreateRoomWithRandomName();
		}
	}
	public void OnJoinRandomRoomButtonClicked() {
		RoomOperationArea.SetActive(false);
		cancelButton.SetActive(true);
		PhotonNetwork.JoinRandomRoom();
	}
	public void OnCancelButtonClicked() {
		Debug.Log("Cancel Button was clicked");
		cancelButton.SetActive(false);
		RoomOperationArea.SetActive(true);
		// TODO this is not working
		PhotonNetwork.LeaveRoom();
	}
}
