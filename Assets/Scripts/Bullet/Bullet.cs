using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour, IPunObservable, IPunInstantiateMagicCallback {
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
	/// <summary>
	/// The damage of this bullet.
	/// </summary>
	public float Damage = 0.0f;
	/// <summary>
	/// Bullets outside of this box will be destroyed.
	/// </summary>
	public Vector3 KillMin = new Vector3(-100.0f, -100.0f, -100.0f);
	/// <summary>
	/// Bullets outside of this box will be destroyed.
	/// </summary>
	public Vector3 KillMax = new Vector3(300.0f, 100.0f, 300.0f);
	/// <summary>
	/// The maximum number of bounces.
	/// </summary>
	public int MaxBounces = 5;
	/// <summary>
	/// The radius of this bullet.
	/// </summary>
	public float Radius = 0.1f;
	/// <summary>
	/// The velocity of this bullet.
	/// </summary>
	public Vector3 Velocity;

	private int _bounces = 0;

	private PhotonView _network;

	void Start() {
		_network = GetComponent<PhotonView>();

		_updateOrientation();
	}

	private void _updateOrientation() {
		VisualTransform.rotation = Quaternion.FromToRotation(new Vector3(0.0f, 1.0f, 0.0f), Velocity);
	}
	private void _emitSparkles(Vector3 pos, Vector3 normal, Vector3 particleVel) {
		PhotonNetwork.Instantiate(
			Path.Combine("Effects", "HitParticles"), pos + 0.1f * normal, Quaternion.FromToRotation(Vector3.forward, particleVel)
		);
	}
	private void _hitPlayer(Component player, Vector3 pos, Vector3 normal, Vector3 particleVel) {
		PhotonView view = player.GetComponent<PhotonView>();
		view.RPC("RPC_OnHit", view.Owner, Damage);
		_emitSparkles(pos, normal, particleVel);
		PhotonNetwork.Destroy(gameObject);
	}

	private void Update() {
		if (_network.IsMine) {
			// check for kill zone
			Vector3 pos = transform.position;
			if (
				pos.x < KillMin.x || pos.x > KillMax.x ||
				pos.y < KillMin.y || pos.y > KillMax.y ||
				pos.z < KillMin.z || pos.z > KillMax.z
			) { // destroy
				PhotonNetwork.Destroy(gameObject);
				return;
			}

			// first test if this bullet is inside any player since SphereCast does not report such intersections
			Collider[] overlapPlayers = Physics.OverlapSphere(transform.position, Radius, 1 << Utils.PlayerLayer);
			if (overlapPlayers.Length > 0) {
				_hitPlayer(overlapPlayers[0], transform.position, -Velocity.normalized, -Velocity);
				return;
			}

			// raycast for physics
			Vector3 dir = Velocity;
			float speed = dir.magnitude;
			dir /= speed;
			if (Physics.SphereCast(
				transform.position, Radius, dir, out RaycastHit hit, speed * Time.deltaTime,
				(1 << Utils.TerrainLayer) | (1 << Utils.PlayerLayer)
			)) {
				if (hit.collider.gameObject.layer == Utils.PlayerLayer) { // hits a player
					_hitPlayer(hit.collider, hit.point, hit.normal, hit.normal);
					return;
				}
				// otherwise hits a wall
				Velocity -= hit.normal * (2.0f * Vector3.Dot(hit.normal, Velocity) / hit.normal.sqrMagnitude);
				transform.position += hit.distance * dir;
				++_bounces;
				// destroy after a certain amount of bounces
				if (_bounces >= MaxBounces) {
					_emitSparkles(hit.point, hit.normal, Velocity);
					PhotonNetwork.Destroy(gameObject);
					return;
				}
			} else {
				transform.position += Velocity * Time.deltaTime;
			}
		}

		// update VisualTransform
		_updateOrientation();
	}

	public void SetColor(Color color, float intensity) {
		Light.color = color;
		Light.intensity = intensity;
		MeshRenderer.material.SetColor("_EmissionColor", color);
	}

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		stream.Serialize(ref Velocity);
	}
	void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info) {
		SetColor(
			new Color(
				(float)info.photonView.InstantiationData[0],
				(float)info.photonView.InstantiationData[1],
				(float)info.photonView.InstantiationData[2],
				(float)info.photonView.InstantiationData[3]
			),
			(float)info.photonView.InstantiationData[4]
		);
		MaxBounces = (int)info.photonView.InstantiationData[5];
	}
}
