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
	/// The height of a level.
	/// </summary>
	public float LevelGap = 1.0f;
	/// <summary>
	/// The speed at which this prism moves.
	/// </summary>
	public float MovingSpeed = 1.0f;
	/// <summary>
	/// The level this prism's at.
	/// </summary>
	private int _targetLevel = 0;
	/// <summary>
	/// Whether or not this prism is currently moving.
	/// </summary>
	private LevelChange _changingLevel = LevelChange.NotChanging;

	/// <summary>
	/// Returns the target vertical position of this prism.
	/// </summary>
	public float TargetLevelPosition {
		get => LevelGap * _targetLevel;
	}
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

	private void FixedUpdate() {
		if (_changingLevel != LevelChange.NotChanging) {
			Vector3 pos = transform.localPosition;
			pos.y += TargetVerticalVelocity * Time.fixedDeltaTime;
			bool
				aboveTarget = pos.y > TargetLevelPosition,
				movingUp = _changingLevel == LevelChange.Upwards;
			if (aboveTarget == movingUp) { // stop
				// snap to position
				pos.y = TargetLevelPosition;
				_changingLevel = LevelChange.NotChanging;
			}
			transform.localPosition = pos;
		}
	}

	/// <summary>
	/// Changes the level of this prism.
	/// </summary>
	/// <param name="levelDelta">The amount of levels to change. This is added to <see cref="_targetLevel"/>.</param>
	public void ChangeLevel(int levelDelta) {
		if (levelDelta != 0) {
			_targetLevel += levelDelta;
			_changingLevel =
				transform.localPosition.y > TargetLevelPosition ? LevelChange.Downwards : LevelChange.Upwards;
		}
	}
}
