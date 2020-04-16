using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour {
	private PhotonView _network;
	private CharacterController _controller;

	/// <summary>
	/// The maximum velocity of the player.
	/// </summary>
	public float MaximumVelocity = 6.0f;
	/// <summary>
	/// Gravity. As large as you desire.
	/// </summary>
	public float Gravity = 30.0f;
	/// <summary>
	/// The maximum acceleration of the player.
	/// </summary>
	public float Acceleration = 10.0f;

	void Start() {
		_network = GetComponent<PhotonView>();
		_controller = GetComponent<CharacterController>();
	}

	void Update() {
		if (_network.IsMine) {
			ProcessTranslation();
		}
	}

	private void ProcessTranslation() {
		// calculate target velocity
		Vector2 targetVelXZ = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		float sqrLen = targetVelXZ.sqrMagnitude;
		if (sqrLen > 1.0f) {
			targetVelXZ /= Mathf.Sqrt(sqrLen);
		}
		targetVelXZ *= MaximumVelocity;

		// calculate new velocity due to acceleration
		Vector3 curVel = _controller.velocity;
		Vector2 curVelXZ = new Vector2(curVel.x, curVel.z);
		Vector2 diff = targetVelXZ - curVelXZ;
		float maxAccel = Acceleration * Time.deltaTime;
		if (diff.sqrMagnitude > maxAccel * maxAccel) {
			targetVelXZ = curVelXZ + diff.normalized * maxAccel;
		}
		curVel.x = targetVelXZ.x;
		curVel.z = targetVelXZ.y;

		// gravity
		if (_controller.isGrounded) {
		}
		curVel.y -= Gravity * Time.deltaTime;

		_controller.Move(curVel * Time.deltaTime);

		/*transform.Translate(Vector3.forward * vertical * zSpeed * Time.deltaTime);//W S 上 下
		transform.Translate(Vector3.right * horizontal * xSpeed * Time.deltaTime);//A D 左右
		//myCC.Move(Vector3.forward * vertical * zSpeed * Time.deltaTime);
		//myCC.Move(Vector3.right * horizontal * xSpeed * Time.deltaTime);*/
	}
}
