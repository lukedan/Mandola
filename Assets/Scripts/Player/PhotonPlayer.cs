using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PhotonPlayer : MonoBehaviour {
	private PhotonView PV;
	public GameObject myAvatar;

	void Start() {
		PV = GetComponent<PhotonView>();
		int spawnPicker = Random.Range(0, GameSetup.GS.spawnPoints.Length);
		if (PV.IsMine) {
			Debug.Log("try to create an avatar");
			myAvatar = PhotonNetwork.Instantiate(Path.Combine("PlayerPrefabs", "PlayerAvatar"),
				GameSetup.GS.spawnPoints[spawnPicker].position, GameSetup.GS.spawnPoints[spawnPicker].rotation, 0);
			if (PV.Owner.IsMasterClient) {

			}
		}
	}
}
