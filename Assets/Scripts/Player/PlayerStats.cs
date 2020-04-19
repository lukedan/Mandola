using Photon.Pun;
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

	private ChromaticAberration _damageEffect;

	private void Start() {
		Camera.main.GetComponent<PostProcessVolume>().profile.TryGetSettings(out _damageEffect);
	}

	private void Update() {
		Health = Mathf.Min(1.0f, Health + HealthRegen * Time.deltaTime);
		float effectTarget = 1.0f - Health;
		_damageEffect.intensity.Override(Mathf.Max(
			effectTarget, _damageEffect.intensity.value - Time.deltaTime * DamageEffectRecovery
		));
	}

	[PunRPC]
	public void OnHit(float damage) {
		Health -= damage;
		_damageEffect.intensity.Override(1.0f - Health + DamageEffectExaggeration);
	}
}
