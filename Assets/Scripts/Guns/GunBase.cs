using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: if you're going to inherit from this, DO NOT reimplement the update() function. Override UpdateGun() and
// Reload() instead.
public abstract class GunBase : MonoBehaviour {
	public bool IsPlayerInShootingMode =>
		!InGameCommon.CurrentGame.IsPaused &&
		transform.parent.GetComponent<PlayerGeneralInfo>().ControlState == PlayerControlState.Shooting;

	private void Update() {
		bool shootingMode = IsPlayerInShootingMode;

		if (shootingMode && Input.GetButton("Reload")) {
			Reload();
		}

		Vector3 aimPos = transform.position;
		if (shootingMode) {
			Ray traceRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(traceRay, out RaycastHit hit, float.PositiveInfinity, 1 << Utils.TerrainLayer)) {
				aimPos = hit.point + new Vector3(0.0f, transform.localPosition.y, 0.0f);
			}
		}
		UpdateGun(aimPos, Time.deltaTime);
	}

	public abstract void Reload();
	public abstract void UpdateGun(Vector3 aim, float deltaTime);
}
