using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagZone : MonoBehaviour {
	/// <summary>
	/// The team that owns this zone.
	/// </summary>
	public int Team = 0;

	private void Start() {
		if (Teams.LevelTeams) {
			var colors = Teams.LevelTeams.Colors;
			if (Team < colors.Count) {
				GetComponent<MeshRenderer>().material.color = colors[Team];
			}
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer == Utils.FlagLayer) {
			PhotonView view = other.GetComponent<PhotonView>();
			view.RPC("RPC_OnFlagCaptured", RpcTarget.AllBufferedViaServer, Team);
			PhotonNetwork.Destroy(other.gameObject);
		}
	}
}
