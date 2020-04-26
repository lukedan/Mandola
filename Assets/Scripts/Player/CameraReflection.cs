using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraReflection : MonoBehaviour {
	/// <summary>
	/// The probe's transform.
	/// </summary>
	public Transform ProbeTransform;

	private void Update() {
		float playerY = 0.0f;
		if (InGameCommon.CurrentGame.MyPlayer) {
			playerY = InGameCommon.CurrentGame.MyPlayer.transform.position.y;
		}
		Vector3 myPos = transform.position;
		ProbeTransform.position = new Vector3(myPos.x, 2.0f * playerY - myPos.y, myPos.z);
	}
}
