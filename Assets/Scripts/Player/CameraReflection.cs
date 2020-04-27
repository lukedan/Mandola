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
		Transform playerTrans = null;
		if (InGameCommon.CurrentGame.MyPlayer) {
			playerTrans = InGameCommon.CurrentGame.MyPlayer.transform;
		}
		float y = 0.0f;
		if (playerTrans) {
			if (Physics.Raycast(
				playerTrans.position + new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, -1.0f, 0.0f),
				out RaycastHit hit, float.MaxValue, 1 << Utils.TerrainLayer
			)) {
				y = hit.point.y;
			}
		}
		Vector3 myPos = transform.position;
		ProbeTransform.position = new Vector3(myPos.x, 2.0f * y - myPos.y, myPos.z);
	}
}
