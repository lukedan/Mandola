using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public enum PlayerControlState {
	Shooting,
	TerrainAlteration
}

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
	/// Health regeneration delay.
	/// </summary>
	public float HealthRegenDelay = 5.0f;
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
	/// The control state of this player.
	/// </summary>
	public PlayerControlState ControlState = PlayerControlState.Shooting;

	/// <summary>
	/// The team this player belongs to.
	/// </summary>
	public int Team = 0;
	public MeshRenderer TeamIndicator;

	private float _healthRegenCd = 0.0f;

	private ChromaticAberration _damageEffect;

	private PhotonView _network;

	public HealthBar healthBarPrefab;

	private HealthBar _healthBar;


	private void Start() {
		_network = GetComponent<PhotonView>();
		InGameCommon.CurrentGame.GlobalPostProcessing.profile.TryGetSettings(out _damageEffect);
		InGameCommon.CurrentGame.PlayerAvatars[_network.CreatorActorNr] = gameObject;
		if (_network.IsMine) {
			CreateHealthBar();
		}
	}

	private void CreateHealthBar() {
		Vector2 screenResolution = new Vector2(Screen.width, Screen.height);
		_healthBar = Instantiate<HealthBar>(healthBarPrefab);
		_healthBar.transform.SetParent(FindObjectOfType<Canvas>().transform);
		_healthBar.transform.position += new Vector3(screenResolution.x, screenResolution.y, 0);
		_healthBar.setHealth(Health);
	}

	private void Update() {
		// health regen
		_healthRegenCd -= Time.deltaTime;
		if (_healthRegenCd < 0.0f) {
			Health = Mathf.Min(1.0f, Health + HealthRegen * Time.deltaTime);
		}

		// update damage effect
		float effectTarget = (1.0f - Health) * DamageEffectAtMinimumHealth;
		_damageEffect.intensity.Override(Mathf.Max(
			effectTarget, _damageEffect.intensity.value - Time.deltaTime * DamageEffectRecovery
		));

		// Update health bar
		if (_healthBar) {
			_healthBar.setHealth(Health);
		}

		// update team indicator
		if (TeamIndicator) {
			if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, float.MaxValue, 1 << Utils.TerrainLayer)) {
				TeamIndicator.transform.localPosition = new Vector3(0.0f, -hit.distance, 0.0f);
			} else {
				TeamIndicator.transform.localPosition = Vector3.zero;
			}
		}
	}

	void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info) {
		Team = (int)info.photonView.InstantiationData[0];
		int characterID = (int)info.photonView.InstantiationData[1];
		Instantiate(PlayerInfo.PI.allCharacters[characterID], transform.position, transform.rotation, transform);

		// set team indicator color
		if (TeamIndicator) {
			Teams teams = InGameCommon.CurrentGame.GetComponent<Teams>();
			if (Team < teams.Colors.Count) {
				TeamIndicator.material.color = teams.Colors[Team];
			} else {
				TeamIndicator.enabled = false;
			}
		}
	}

	/// <summary>
	/// Called when the player is hit. This will only be called for the affected player for its owner client.
	/// </summary>
	[PunRPC]
	public void RPC_OnHit(float damage) {
		Debug.Assert(_network.IsMine);
		_healthRegenCd = HealthRegenDelay;
		Health -= damage;
		_damageEffect.intensity.Override(Mathf.Min(
			_damageEffect.intensity.value + DamageEffectExaggeration, DamageEffectClamp
		));
		if (Health <= 0.0f) {
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
