using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class PhotonLobby : MonoBehaviourPunCallbacks {
	public static PhotonLobby lobby;

	public const string RespawnDelayPropertyName = "respawnDelay";

	public GameObject ConnectingIndicator;
	public GameObject RoomOperationArea;
	public GameObject cancelButton;
	public InputField RoomNameInput;

	public GameObject RoomListPanel;
	public GameObject RoomItemPrefab;

	public string SelectedGameModeSceneName; // TODO this should be a room attribute about which map is being played

	private struct RoomDisplayInfo {
		public RoomInfo Info;
		public RoomItemDisplay Display;
	}

	private Dictionary<string, RoomDisplayInfo> _cachedRooms = new Dictionary<string, RoomDisplayInfo>();

	private void Awake() {
		lobby = this;  // Create the singleton, lives withing the main menu scene
	}

	private void Start() {
		if (!PhotonNetwork.IsConnected) {
			PhotonNetwork.ConnectUsingSettings();  // Connects to Master photon server.
		} else if (!PhotonNetwork.InLobby) {
			PhotonNetwork.JoinLobby();
		} else {
			_LobbyJoined();
		}
	}

#if DEBUG
	private void OnGUI() {
		GUILayout.Label(PhotonNetwork.NetworkStatisticsToString());
		GUILayout.Label(PhotonNetwork.CurrentCluster);
	}
#endif

	public override void OnConnectedToMaster() {
		Debug.Log("player has connected to the Photon master server");
		PhotonNetwork.AutomaticallySyncScene = true;
		PhotonNetwork.JoinLobby();
	}
	public override void OnJoinedLobby() {
		base.OnJoinedLobby();
		_LobbyJoined();
	}
	public override void OnJoinedRoom() {
		base.OnJoinedRoom();
		PhotonNetwork.LoadLevel(SelectedGameModeSceneName);
	}
	public override void OnCreateRoomFailed(short returnCode, string message) {
		Debug.Log("Try to create a new room but failed. There must be alread a room with the same name");
		_CreateRoomWithRandomName();
	}
	public override void OnJoinRandomFailed(short returnCode, string message) {
		Debug.Log("Try to join a game but failed. There must be no open game available");
		_CreateRoomWithRandomName();
	}
	public override void OnRoomListUpdate(List<RoomInfo> roomList) {
		base.OnRoomListUpdate(roomList);
		foreach (RoomInfo room in roomList) {
			if (room.RemovedFromList) {
				if (_cachedRooms.TryGetValue(room.Name, out RoomDisplayInfo info)) {
					if (info.Display) {
						Destroy(info.Display.gameObject);
					}
				}
				_cachedRooms.Remove(room.Name);
			} else {
				RoomDisplayInfo info;
				if (!_cachedRooms.TryGetValue(room.Name, out info)) {
					info = new RoomDisplayInfo();
				}
				info.Info = room;

				if (room.IsVisible) {
					bool manualRefresh = true;
					if (!info.Display) {
						GameObject item = Instantiate(RoomItemPrefab);
						item.transform.parent = RoomListPanel.transform;
						item.GetComponent<Button>().onClick.AddListener(() => {
							_OnStartedJoiningRoom();
							PhotonNetwork.JoinRoom(room.Name);
						});
						info.Display = item.GetComponent<RoomItemDisplay>();
						manualRefresh = false;
					}
					info.Display.Name = room.Name;
					info.Display.NumPlayers = room.PlayerCount;
					info.Display.MaxNumPlayers = room.MaxPlayers;
					if (manualRefresh) {
						info.Display.Refresh();
					}
				} else {
					if (info.Display) {
						Destroy(info.Display.gameObject);
						info.Display = null;
					}
				}

				_cachedRooms[room.Name] = info;
			}
		}
	}

	private	void _OnStartedJoiningRoom() {
		RoomOperationArea.SetActive(false);
		cancelButton.SetActive(true);
	}
	private void _CreateRoom(string name) {
		Debug.Log("trying to create room " + name);
		_OnStartedJoiningRoom();
		RoomOptions roomOps = new RoomOptions() {
			IsVisible = true,
			IsOpen = true,
			MaxPlayers = 6
		};
		roomOps.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
		roomOps.CustomRoomProperties.Add(RespawnDelayPropertyName, 5.0f);
		PhotonNetwork.CreateRoom(name, roomOps);
	}
	private void _CreateRoomWithRandomName() {
		_CreateRoom("Room " + Random.Range(0, 10000).ToString());
	}
	private	void _LobbyJoined() {
		ConnectingIndicator.SetActive(false);
		RoomOperationArea.SetActive(true);
	}

	public void OnCreateRoomButtonClicked() {
		if (RoomNameInput.text.Length > 0) {
			_CreateRoom(RoomNameInput.text);
		} else {
			_CreateRoomWithRandomName();
		}
	}
	public void OnJoinRandomRoomButtonClicked() {
		_OnStartedJoiningRoom();
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
