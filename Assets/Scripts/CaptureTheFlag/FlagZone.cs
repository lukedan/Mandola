using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagZone : MonoBehaviour {
	/// <summary>
	/// The team that owns this zone.
	/// </summary>
	public int Team = 0;

	private PhotonView _sceneNetwork;

	private void Start() {
		_sceneNetwork = InGameCommon.CurrentGame.GetComponent<PhotonView>();
		Teams level = InGameCommon.CurrentGame.GetComponent<Teams>();
		List<Color> colors = level.Colors;
		if (Team < colors.Count) {
			GetComponent<MeshRenderer>().material.color = colors[Team];
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer == Utils.FlagLayer) {
			_sceneNetwork.RPC("RPC_OnScore", RpcTarget.AllBufferedViaServer, Team, 1);
			PhotonNetwork.Destroy(other.gameObject);
		}
	}
}
