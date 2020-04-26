using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraReflection : MonoBehaviour {
	/// <summary>
	/// The transform of the player.
	/// </summary>
	public Transform PlayerTransform;
	/// <summary>
	/// The probe's transform.
	/// </summary>
	public Transform ProbeTransform;

	private void Start() {
		if (!transform.GetComponent<PlayerCamera>().playerTransform.GetComponent<PhotonView>().IsMine) {
			enabled = false;
			ProbeTransform.GetComponent<ReflectionProbe>().enabled = false;
		}
	}

	private void Update() {
		Vector3 myPos = transform.position;
		float playerY = PlayerTransform.position.y;
		ProbeTransform.position = new Vector3(myPos.x, 2.0f * playerY - myPos.y, myPos.z);
	}
}
