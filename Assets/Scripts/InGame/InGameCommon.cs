using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameCommon : MonoBehaviourPunCallbacks {
	public static InGameCommon CurrentGame;

	public string SceneOnLeftRoom = "NetworkScene";

	private void Start() {
		CurrentGame = this;
	}

	public override void OnLeftRoom() {
		SceneManager.LoadScene(SceneOnLeftRoom);
	}
}
