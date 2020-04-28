using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
	public GameObject PausedMenuUI;

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (InGameCommon.CurrentGame.IsPaused) {
				ResumeGame();
			} else {
				InGameCommon.CurrentGame.IsPaused = true;
				PausedMenuUI.SetActive(true);
			}
		}
	}

	// Make the pause menu invisible to resume the game
	public void ResumeGame() {
		InGameCommon.CurrentGame.IsPaused = false;
		PausedMenuUI.SetActive(false);
	}

	// Return to the Lobby, i.e., the NetworkScene to join a new room
	public void BackToLobby() {
		InGameCommon.CurrentGame.SceneOnLeftRoom = "NetworkScene";
		PhotonNetwork.LeaveRoom();
	}

	// Return to the start menu
	public void ExitGame() {
		InGameCommon.CurrentGame.SceneOnLeftRoom = "StartScene";
		PhotonNetwork.LeaveRoom();
	}
}
