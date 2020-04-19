using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Bullet : MonoBehaviour {
	/// <summary>
	/// The color of this bullet.
	/// </summary>
	public Color BulletColor;
	/// <summary>
	/// The color intensity of this bullet.
	/// </summary>
	public float BulletColorIntensity = 1.0f;
	/// <summary>
	/// The light.
	/// </summary>
	public Light Light;
	/// <summary>
	/// The mesh renderer.
	/// </summary>
	public MeshRenderer MeshRenderer;
	/// <summary>
	/// The transform of the visuals of this object.
	/// </summary>
	public Transform VisualTransform;

	private Rigidbody _rigidBody;
	private int _bounces = 0;

	private PhotonView PV;

	void Start() {
		PV = GetComponent<PhotonView>();
		_rigidBody = GetComponent<Rigidbody>();

		Light.color = BulletColor;
		Light.intensity = BulletColorIntensity;

		MeshRenderer.material.SetColor("_EmissionColor", BulletColor);

		_updateOrientation();
	}

	private void _updateOrientation() {
		VisualTransform.rotation = Quaternion.FromToRotation(new Vector3(0.0f, 1.0f, 0.0f), _rigidBody.velocity);
	}

	void Update() {
		// update VisualTransform
		_updateOrientation();
	}

	private void OnCollisionEnter(Collision collision) {
		// TODO detect collision with player
		++_bounces;
	}
}
