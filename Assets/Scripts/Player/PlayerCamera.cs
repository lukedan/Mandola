using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {
	//Camera offset
	private Vector3 offsetValue;
	private float offsetScaleX;
	private float offsetScaleZ;

	//Initial offset between camera and avatar
	private Vector3 currentOffset = new Vector3(0.0f, 0.0f, 0.0f);

	//Initial position offset between avatar and camera
	public Vector3 initialPositionOffset = new Vector3(0, 23, -15);
	public Vector3 initialRotation = new Vector3(53, 0, 0);

	//Scalar, to control the velocity of camera shift
	public float shiftingVelocityScalar = 0.8f;

	private void Start() {
		offsetValue = new Vector3(13, 0, 10);
	}

	private void Update() {
		Transform playerTransform = null;
		if (InGameCommon.CurrentGame.MyPlayer) {
			playerTransform = InGameCommon.CurrentGame.MyPlayer.transform;
		}
		if (playerTransform) {
			Vector2 screenResolution = new Vector2(Screen.width, Screen.height);

			// Clamp the mouse position within the resolution of current window
			float mousePosX = Mathf.Clamp(Input.mousePosition.x, 0, screenResolution.x);
			float mousePosY = Mathf.Clamp(Input.mousePosition.y, 0, screenResolution.y);

			offsetScaleX = ((mousePosX - screenResolution.x * 0.5f) / (screenResolution.x * 0.5f));
			offsetScaleZ = ((mousePosY - screenResolution.y * 0.5f) / (screenResolution.y * 0.5f));

			Vector3 targetOffset = new Vector3(offsetScaleX * offsetValue.x, 0, offsetScaleZ * offsetValue.z);

			Vector3 velocity = (targetOffset - currentOffset) * shiftingVelocityScalar;

			currentOffset += velocity * Time.deltaTime;

			transform.position = playerTransform.position + initialPositionOffset + currentOffset;
			transform.rotation = Quaternion.Euler(initialRotation);
		}
	}
}