using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UIElements;

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
	private Vector3 _prevPosition;
	private float _radius = 0.0f;

	private PhotonView PV;

	void Start() {
		PV = GetComponent<PhotonView>();
		_rigidBody = GetComponent<Rigidbody>();

		Light.color = BulletColor;
		Light.intensity = BulletColorIntensity;
		MeshRenderer.material.SetColor("_EmissionColor", BulletColor);

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
		// raycast for player hits
		if (PV.IsMine) {
			Vector3 pos = transform.position;
			Collider[] cols = Physics.OverlapCapsule(_prevPosition, pos, _radius, 1 << Utils.PlayerLayer);
			if (cols.Length > 0) {
				Collider minCol = cols[0];
				float minDist = float.MaxValue;
				foreach (Collider c in cols) {
					float dist = (c.transform.position - pos).sqrMagnitude;
					if (dist < minDist) {
						minDist = dist;
						minCol = c;
					}
				}
				// TODO send message to minCol
				PhotonNetwork.Destroy(gameObject);
			}
		}
	}

	private void OnCollisionEnter(Collision collision) {
		++_bounces;
	}
}
