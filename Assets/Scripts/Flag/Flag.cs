﻿using Photon.Pun;
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
		if (_network.IsMine && transform.parent == null) {
			if (other.gameObject.layer == Utils.PlayerLayer) {
				PhotonView collidingView = other.GetComponent<PhotonView>();
				if (!collidingView.Owner.IsLocal) {
					_network.TransferOwnership(collidingView.Owner);
				}
				collidingView.RPC("RPC_OnPlayerGotFlag", RpcTarget.All, _network.ViewID);
			}
		}
	}

	[PunRPC]
	public void RPC_OnPlayerKilled() {
		transform.parent = null;
		GetComponent<Rigidbody>().isKinematic = false;
	}
	[PunRPC]
	public void RPC_OnFlagCaptured(int team) {
		Teams.LevelTeams.OnScore(team);
	}
}
