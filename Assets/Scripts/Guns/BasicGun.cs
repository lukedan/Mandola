using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGun : GunBase {
	// config variables
	/// <summary>
	/// The number of bullets in a clip.
	/// </summary>
	public int ClipSize = 30;

	/// <summary>
	/// If this is true, then the user must release the mouse button after one shot before another shot can be fired.
	/// </summary>
	public bool SemiAuto = false;
	/// <summary>
	/// The delay between two consecutive shots.
	/// </summary>
	public float FiringDelay = 0.1f;
	/// <summary>
	/// The time it takes to reload.
	/// </summary>
	public float ReloadTime = 3.0f;

	/// <summary>
	/// The distance with respect to which the accuracy is computed.
	/// </summary>
	public float AccuracyDistance = 10.0f;
	/// <summary>
	/// Inaccuracy caused by each shot.
	/// </summary>
	public float ShootInaccuracy = 0.1f;
	/// <summary>
	/// Maximum inaccuracy of this weapon.
	/// </summary>
	public float MaximumInaccuracy = 2.0f;
	/// <summary>
	/// The rate at which accuracy restores.
	/// </summary>
	public float AccuracyRestore = 1.0f;

	/// <summary>
	/// The color of this bullet.
	/// </summary>
	public Color BulletColor;
	/// <summary>
	/// The color intensity of this bullet.
	/// </summary>
	public float BulletColorIntensity = 1.0f;
	/// <summary>
	/// Bullet damage.
	/// </summary>
	public float BulletDamage = 0.1f;
	/// <summary>
	/// The bullet. It must have a rigid body.
	/// </summary>
	public GameObject BulletPrefab;
	/// <summary>
	/// The speed of fired bullets.
	/// </summary>
	public float BulletSpeed = 50.0f;
	/// <summary>
	/// The initial offset of the spawned bullet with respect to the player. This is used to prevent the bullet from
	/// intersecting with the player himself.
	/// </summary>
	public float FireOffset = 1.0f;


	// state variables
	/// <summary>
	/// The number of reserve bullets, i.e., bullets that are not in the clip.
	/// </summary>
	public int NumReserveBullets = 200;
	private int _numClipBullets = 0;

	private float _inaccuracy = 0.0f;

	private float _reloadCooldown = -1.0f;
	private float _fireCooldown = 0.0f;
	private bool _freshTrigger = true;

	private CharacterController _parentVelocity;
	private LineRenderer _aim;

	private PhotonView _network;


	void Start() {
		Transform parent = transform.parent;
		_network = parent.GetComponent<PhotonView>();
		if (!_network.IsMine) {
			enabled = false;
			return;
		}
		_parentVelocity = parent.GetComponent<CharacterController>();
		_aim = GetComponent<LineRenderer>();

		_instantReload();
	}

	/// <summary>
	/// Instantly reloads.
	/// </summary>
	private void _instantReload() {
		int totalBullets = NumReserveBullets + _numClipBullets;
		int count = Math.Min(ClipSize, totalBullets);
		_numClipBullets = count;
		NumReserveBullets = totalBullets - count;
	}

	public bool IsReloading => _reloadCooldown > 0.0f;
	public bool CanFire => _numClipBullets > 0 && _fireCooldown <= 0.0f && !(SemiAuto && !_freshTrigger);

	public override void Reload() {
		// cannot reload while reloading
		// cannot reload without reserve bullets
		if (!IsReloading && NumReserveBullets > 0) {
			_reloadCooldown = ReloadTime;
		}
	}
	public override void UpdateGun(Vector3 aim, float deltaTime) {
		Vector3 position = transform.position;
		if (_aim) {
			Vector3 diff = aim - position;
			float length = diff.magnitude;
			diff /= length;
			length = Mathf.Min(2.0f, 0.5f * length);
			_aim.SetPosition(0, position);
			_aim.SetPosition(1, position + length * diff);
			_aim.SetPosition(2, aim - length * diff);
			_aim.SetPosition(3, aim);
		}

		if (IsReloading) {
			_reloadCooldown -= deltaTime;
			if (!IsReloading) {
				_instantReload();
				_fireCooldown = 0.0f;
				_inaccuracy = 0.0f;
			}
		} else { // try to fire
			_fireCooldown -= deltaTime;
			_inaccuracy = Mathf.Max(0.0f, _inaccuracy - deltaTime * AccuracyRestore);

			bool firing = Input.GetButton("Fire");
			if (firing) {
				while (CanFire) {
					_fireCooldown += FiringDelay;
					_freshTrigger = false;

					// fire bullet
					Vector3 direction = (aim - position).normalized;
					direction = direction * AccuracyDistance + UnityEngine.Random.insideUnitSphere * _inaccuracy;
					direction = direction.normalized;
					GameObject bullet = PhotonNetwork.Instantiate(
						"Bullet", position + direction * FireOffset, Quaternion.identity, 0,
						new object[] {
							BulletColor.r, BulletColor.g, BulletColor.b, BulletColor.a, BulletColorIntensity
						}
					);
					// TODO if the players are not happy with this non-WYSIWYG velocity, compensate for player velocity
					bullet.GetComponent<Rigidbody>().velocity = direction * BulletSpeed + _parentVelocity.velocity;
					bullet.GetComponent<Bullet>().Damage = BulletDamage;

					_inaccuracy = Mathf.Min(MaximumInaccuracy, _inaccuracy + ShootInaccuracy);
					--_numClipBullets;
				}
				if (_numClipBullets == 0) {
					Reload();
				}
			} else {
				_fireCooldown = Mathf.Max(_fireCooldown, 0.0f);
				_freshTrigger = true;
			}
		}
	}
}
