﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerStats : MonoBehaviour {
	/// <summary>
	/// Player health.
	/// </summary>
	public float Health = 1.0f;
	/// <summary>
	/// Health regeneration rate.
	/// </summary>
	public float HealthRegen = 0.1f;
	/// <summary>
	/// A parameter of the damage effect.
	/// </summary>
	public float DamageEffectExaggeration = 0.3f;
	/// <summary>
	/// The duration of damage effect.
	/// </summary>
	public float DamageEffectRecovery = 3.0f;
	/// <summary>
	/// The maximum damage effect value, except for when it's exaggerated after being hit.
	/// </summary>
	public float DamageEffectAtMinimumHealth = 1.0f;
	/// <summary>
	/// Clamp the effect below this value.
	/// </summary>
	public float DamageEffectClamp = 3.0f;

	/// <summary>
	/// The team this player belongs to.
	/// </summary>
	public int Team = 0;

	/// <summary>
	/// The post processing volume.
	/// </summary>
	public PostProcessVolume PostProcessVolume;

	private ChromaticAberration _damageEffect;

	private PhotonView _network;

	private void Start() {
		_network = GetComponent<PhotonView>();
		if (!_network.IsMine) {
			PostProcessVolume.enabled = false;
		}
		PostProcessVolume.profile.TryGetSettings(out _damageEffect);
	}

	private void Update() {
		Health = Mathf.Min(1.0f, Health + HealthRegen * Time.deltaTime);
		float effectTarget = (1.0f - Health) * DamageEffectAtMinimumHealth;
		_damageEffect.intensity.Override(Mathf.Max(
			effectTarget, _damageEffect.intensity.value - Time.deltaTime * DamageEffectRecovery
		));
	}

	/// <summary>
	/// Called when the player is hit. This will only be called for the affected player for its owner client.
	/// </summary>
	[PunRPC]
	public void RPC_OnHit(float damage) {
		Debug.Assert(_network.IsMine);
		Health -= damage;
		float val = _damageEffect.intensity.value;
		_damageEffect.intensity.Override(Mathf.Min(val + DamageEffectExaggeration, DamageEffectClamp));
		if (Health < 0.0f) {
			// player is dead
			// drop flag
			Flag flag = GetComponentInChildren<Flag>();
			if (flag) {
				flag.GetComponent<PhotonView>().RPC("RPC_OnPlayerKilled", RpcTarget.All);
			}
			// TODO

			//Detach camera from avatar
			//This should be placed before destroy the avatar
			Camera avatarCamera = gameObject.transform.GetChild(1).gameObject.GetComponent<Camera>();
			avatarCamera.GetComponent<PlayerCamera>().DetachCameraFromAvatar();

			//Destroy the avatar
			PhotonView.Destroy(gameObject);
		}
	}

	[PunRPC]
	public void RPC_OnPlayerGotFlag(int flagObjectID) {
		// attach flag to this player
		PhotonView flagView = PhotonView.Find(flagObjectID);
		flagView.transform.parent = transform;
		flagView.transform.localPosition = new Vector3(0.0f, 3.0f, 0.0f);
		flagView.GetComponent<Rigidbody>().isKinematic = true;
	}
}
