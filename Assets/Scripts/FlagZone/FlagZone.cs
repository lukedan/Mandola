using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagZone : MonoBehaviour {
	/// <summary>
	/// The team that owns this zone.
	/// </summary>
	public int Team = 0;

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer == Utils.FlagLayer) {
			// TODO
			PhotonNetwork.Destroy(other.gameObject);
		}
	}
}
