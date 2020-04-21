using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FlagSpawn : MonoBehaviour {
	private void Start() {
		if (GetComponent<PhotonView>().IsMine) {
			PhotonNetwork.Instantiate(Path.Combine("Flag", "Flag"), transform.position, transform.rotation);
		}
	}
}
