using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FlagSpawn : MonoBehaviour {
	private PhotonView _network;
	private Flag _spawn;

	private void Start() {
		_network = GetComponent<PhotonView>();
	}

	private void Update() {
		if (_network.IsMine) {
			if (_spawn == null) {
				_spawn = PhotonNetwork.Instantiate(
					Path.Combine("CaptureTheFlag", "Flag"), transform.position, transform.rotation
				).GetComponent<Flag>();
			}
		}
	}
}
