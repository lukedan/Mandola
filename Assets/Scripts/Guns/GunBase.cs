using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: if you're going to inherit from this, DO NOT reimplement the update() function. Override UpdateGun() and
// Reload() instead.
public abstract class GunBase : MonoBehaviour {
	private void Update() {
		if (Input.GetButton("Reload")) {
			Reload();
		}

		Vector3 aimPos = new Vector3();
		Ray traceRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(traceRay, out RaycastHit hit, float.PositiveInfinity, 1 << Utils.TerrainLayer)) {
			aimPos = hit.point + new Vector3(0.0f, transform.localPosition.y, 0.0f);
		}
		UpdateGun(aimPos, Time.deltaTime);
	}

	public abstract void Reload();
	public abstract void UpdateGun(Vector3 aim, float deltaTime);
}
