using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
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

	private Rigidbody _rigidBody;
	private int _bounces = 0;
	private Vector3 _prevPosition;
	private float _radius = 0.0f;

	private PhotonView _network;

	void Start() {
		_network = GetComponent<PhotonView>();

		_rigidBody = GetComponent<Rigidbody>();

		_prevPosition = transform.position;
		_radius = GetComponent<SphereCollider>().radius;

		_updateOrientation();
	}

	private void _updateOrientation() {
		VisualTransform.rotation = Quaternion.FromToRotation(new Vector3(0.0f, 1.0f, 0.0f), _rigidBody.velocity);
	}

	private void Update() {
		// update VisualTransform
		_updateOrientation();
	}

	private void FixedUpdate() {
		if (_network.IsMine) {
			// raycast for player hits
			Vector3 pos = transform.position;

			// check for kill zone
			if (
				pos.x < KillMin.x || pos.x > KillMax.x ||
				pos.y < KillMin.y || pos.y > KillMax.y ||
				pos.z < KillMin.z || pos.z > KillMax.z
			) { // destroy
				PhotonNetwork.Destroy(gameObject);
				return;
			}

			Collider[] cols = Physics.OverlapCapsule(_prevPosition, pos, _radius, 1 << Utils.PlayerLayer);
			if (cols.Length > 0) {
				Collider minCol = null;
				float minDist = float.MaxValue;
				foreach (Collider c in cols) {
					float dist = (c.transform.position - _prevPosition).sqrMagnitude;
					if (dist < minDist) {
						minDist = dist;
						minCol = c;
					}
				}
				if (minCol) {
					PhotonView view = minCol.GetComponent<PhotonView>();
					view.RPC("RPC_OnHit", view.Owner, Damage);
					PhotonNetwork.Destroy(gameObject);
				}
			}
			_prevPosition = pos;
		}
	}

	private void OnCollisionEnter(Collision collision) {
		if (_network.IsMine) {
			++_bounces;
			// destroy after a certain amount of bounces
			if (_bounces >= MaxBounces) {
				PhotonNetwork.Destroy(gameObject);
			}
		}
	}

	public void SetColor(Color color, float intensity) {
		Light.color = color;
		Light.intensity = intensity;
		MeshRenderer.material.SetColor("_EmissionColor", color);
	}

	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (_rigidBody) {
			if (stream.IsWriting) {
				stream.SendNext(_rigidBody.velocity);
			} else {
				_rigidBody.velocity = (Vector3)stream.ReceiveNext();
			}
		}
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
