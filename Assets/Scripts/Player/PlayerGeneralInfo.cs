using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerGeneralInfo : MonoBehaviour, IPunInstantiateMagicCallback {
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

	private ChromaticAberration _damageEffect;

	private PhotonView _network;

	private void Start() {
		_network = GetComponent<PhotonView>();
		InGameCommon.CurrentGame.GlobalPostProcessing.profile.TryGetSettings(out _damageEffect);
		InGameCommon.CurrentGame.PlayerAvatars[_network.Owner.ActorNumber] = gameObject;
	}

	private void Update() {
		Health = Mathf.Min(1.0f, Health + HealthRegen * Time.deltaTime);
		float effectTarget = (1.0f - Health) * DamageEffectAtMinimumHealth;
		_damageEffect.intensity.Override(Mathf.Max(
			effectTarget, _damageEffect.intensity.value - Time.deltaTime * DamageEffectRecovery
		));
	}

	void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info) {
		Team = (int)info.photonView.InstantiationData[0];
		int characterID = (int)info.photonView.InstantiationData[1];
		Instantiate(PlayerInfo.PI.allCharacters[characterID], transform.position, transform.rotation, transform);
	}

	/// <summary>
	/// Called when the player is hit. This will only be called for the affected player for its owner client.
	/// </summary>
	[PunRPC]
	public void RPC_OnHit(float damage) {
		Debug.Assert(_network.IsMine);
		Health -= damage;
		_damageEffect.intensity.Override(Mathf.Min(
			_damageEffect.intensity.value + DamageEffectExaggeration, DamageEffectClamp
		));
		if (Health < 0.0f) {
			// drop flag
			Flag flag = GetComponentInChildren<Flag>();
			if (flag) {
				flag.GetComponent<PhotonView>().RPC("RPC_OnPlayerKilled", RpcTarget.All);
			}
			// destroy the player
			PhotonNetwork.Destroy(gameObject);
			InGameCommon.CurrentGame.OnPlayerKilled();
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
