using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AnimStatus {
	IDLE,
	MOVE,
	MOVE_SHOOT,
	JUMP,
	SHOOT,
	RECHARGE
};

public class RobotMovement : MonoBehaviour {
	private Animator _animator;
	private CharacterController _move;

	private void Start() {
		_animator = GetComponent<Animator>();

		Transform parent = transform.parent;
		_move = parent.GetComponent<CharacterController>();

		Teams teams = InGameCommon.CurrentGame.GetComponent<Teams>();
		transform.GetChild(7).GetComponent<MeshRenderer>().material.color = teams.Colors[InGameCommon.CurrentGame.PlayerTeam];
		Debug.Log("=====" + InGameCommon.CurrentGame.PlayerTeam);
	}

	// Update is called once per frame
	void Update() {
		GunBase gun = transform.parent.GetComponentInChildren<GunBase>();
		Vector3 aimDir = gun.Aim - transform.parent.position;
		Vector3 vel = _move.velocity;
		Vector2 aimDirXZ = new Vector2(aimDir.x, aimDir.z).normalized;
		Vector2 velXZ = new Vector2(vel.x, vel.z);
		float forward = Vector2.Dot(aimDirXZ, velXZ);
		float strafe = Vector2.Dot(new Vector2(aimDirXZ.y, -aimDirXZ.x), velXZ);
		_animator.SetFloat("Forward", forward);
		_animator.SetFloat("Strafe", strafe);
		_animator.SetBool("Grounded", _move.isGrounded);
		_animator.SetBool("Reloading", gun.IsReloading);

		transform.localRotation = Quaternion.FromToRotation(new Vector3(0.0f, 0.0f, 1.0f), new Vector3(aimDirXZ.x, 0.0f, aimDirXZ.y));
	}
}

