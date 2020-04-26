using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour {
	private PhotonView _network;

	private void Start() {
		_network = GetComponent<PhotonView>();
	}

	private void OnTriggerEnter(Collider other) {
		if (_network.IsMine) {
			if (!transform.parent) {
				if (other.gameObject.layer == Utils.PlayerLayer) {
					PhotonView collidingView = other.GetComponent<PhotonView>();
					if (!collidingView.IsMine) {
						_network.TransferOwnership(collidingView.Owner);
					}
					collidingView.RPC("RPC_OnPlayerGotFlag", RpcTarget.All, _network.ViewID);
				}
			}
			if (other.gameObject.layer == Utils.FlagZoneLayer) {
				InGameCommon.CurrentGame.GetComponent<PhotonView>().RPC(
					"RPC_OnFlagCaptured", RpcTarget.AllBufferedViaServer,
					other.GetComponent<FlagZone>().Team, _network.ViewID
				);
				GetComponent<Collider>().enabled = false;
			}
		}
	}

	[PunRPC]
	public void RPC_OnPlayerKilled() {
		transform.parent = null;
		GetComponent<Rigidbody>().isKinematic = false;
	}
}
