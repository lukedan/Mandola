using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedDestroyAfterTime : MonoBehaviour {
	/// <summary>
	/// The amount of time before this is destroyed.
	/// </summary>
	public float Countdown = 1.0f;

	private void Start() {
		PhotonView view = GetComponent<PhotonView>();
		if (!view || !view.IsMine) {
			enabled = false;
		}
	}

	private void Update() {
		Countdown -= Time.deltaTime;
		if (Countdown < 0.0f) {
			PhotonView view = GetComponent<PhotonView>();
			if (view && view.IsMine) {
				PhotonNetwork.Destroy(gameObject);
			}
		}
	}
}
