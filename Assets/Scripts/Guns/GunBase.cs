using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: if you're going to inherit from this, DO NOT reimplement the update() function. Override UpdateGun() and
// Reload() instead.
public abstract class GunBase : MonoBehaviour {
	public string Identifier;

	public bool IsPlayerInShootingMode =>
		!InGameCommon.CurrentGame.IsPaused &&
		transform.parent.GetComponent<PlayerGeneralInfo>().ControlState == PlayerControlState.Shooting;

	public Vector3 Aim { get; private set; }
	public bool IsReloading => GetIsReloading();
	public abstract bool GetIsReloading();

	private void Update() {
		bool shootingMode = IsPlayerInShootingMode;

		if (shootingMode && Input.GetButton("Reload")) {
			Reload();
		}

		Aim = transform.position;
		Ray traceRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(traceRay, out RaycastHit hit, float.PositiveInfinity, 1 << Utils.TerrainLayer)) {
			Aim = hit.point + new Vector3(0.0f, transform.localPosition.y, 0.0f);
		}
		UpdateGun(Time.deltaTime);
	}

	public abstract void Reload();
	public abstract void UpdateGun(float deltaTime);
	public virtual void OnDestroying() {
		// nothing to do
	}
}
