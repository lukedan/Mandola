using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrismMover : MonoBehaviour {
	/// <summary>
	/// Indicates the direction in which this prism is moving.
	/// </summary>
	private enum LevelChange {
		/// <summary>
		/// This prism is not moving.
		/// </summary>
		NotChanging,
		/// <summary>
		/// This prism is moving upwards.
		/// </summary>
		Upwards,
		/// <summary>
		/// This prism is moving downwards.
		/// </summary>
		Downwards
	}

	/// <summary>
	/// The speed at which this prism moves.
	/// </summary>
	public float MovingSpeed = 1.0f;
	/// <summary>
	/// Maximum height.
	/// </summary>
	public float MaxHeight = 4.0f;
	/// <summary>
	/// Minimum height.
	/// </summary>
	public float MinHeight = -4.0f;
	/// <summary>
	/// The target height of this prism.
	/// </summary>
	private float _targetHeight = 0.0f;
	/// <summary>
	/// Whether or not this prism is currently moving.
	/// </summary>
	private LevelChange _changingLevel = LevelChange.NotChanging;

	/// <summary>
	/// Target vertical velocity of this prism.
	/// </summary>
	public float TargetVerticalVelocity {
		get {
			switch (_changingLevel) {
				case LevelChange.Upwards:
					return MovingSpeed;
				case LevelChange.Downwards:
					return -MovingSpeed;
				default:
					return 0.0f;
			}
		}
	}

	private void Update() {
		if (_changingLevel != LevelChange.NotChanging) {
			Vector3 pos = transform.localPosition;
			pos.y += TargetVerticalVelocity * Time.deltaTime;
			bool
				aboveTarget = pos.y > _targetHeight,
				movingUp = _changingLevel == LevelChange.Upwards;
			if (aboveTarget == movingUp) { // stop
				// snap to position
				pos.y = _targetHeight;
				_changingLevel = LevelChange.NotChanging;
			}
			transform.localPosition = pos;
		}
	}

	/// <summary>
	/// Changes the level of this prism.
	/// </summary>
	/// <param name="levelDelta">The amount of levels to change. This is added to <see cref="_targetLevel"/>.</param>
	public void ChangeHeight(float delta) {
		_targetHeight = Mathf.Clamp(_targetHeight + delta, MinHeight, MaxHeight);
		_changingLevel =
			transform.localPosition.y > _targetHeight ? LevelChange.Downwards : LevelChange.Upwards;
	}

	/// <summary>
	/// Immediately changes the height of this prism.
	/// </summary>
	/// <param name="delta"></param>
	public void ChangeHeightImmediate(float delta) {
		_targetHeight = Mathf.Clamp(_targetHeight + delta, MinHeight, MaxHeight);
		_changingLevel = LevelChange.NotChanging;
		Vector3 pos = transform.localPosition;
		pos.y = _targetHeight;
		transform.localPosition = pos;
	}
}
