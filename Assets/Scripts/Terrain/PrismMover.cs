using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

	public Color MaxHeightCapColor {
		get => Color.HSVToRGB(_maxHeightCapColorHSV.x, _maxHeightCapColorHSV.y, _maxHeightCapColorHSV.z);
		set {
			Color.RGBToHSV(value, out _maxHeightCapColorHSV.x, out _maxHeightCapColorHSV.y, out _maxHeightCapColorHSV.z);
		}
	}
	private Vector3 _maxHeightCapColorHSV;
	public Color MinHeightCapColor {
		get => Color.HSVToRGB(_minHeightCapColorHSV.x, _minHeightCapColorHSV.y, _minHeightCapColorHSV.z);
		set {
			Color.RGBToHSV(value, out _minHeightCapColorHSV.x, out _minHeightCapColorHSV.y, out _minHeightCapColorHSV.z);
		}
	}
	private Vector3 _minHeightCapColorHSV;

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

	private void _UpdateCapEmission(float pos) {
		float t = (pos - MinHeight) / (MaxHeight - MinHeight);
		Vector3 hsv = (1.0f - t) * _minHeightCapColorHSV + t * _maxHeightCapColorHSV;
		if (Mathf.Abs(_minHeightCapColorHSV.x - _maxHeightCapColorHSV.x) > 0.5f) {
			hsv.x = hsv.x > 0.5f ? hsv.x - 0.5f : hsv.x + 0.5f;
		}
		GetComponent<MeshRenderer>().material.SetColor("_CapEmission", Color.HSVToRGB(hsv.x, hsv.y, hsv.z));
	}

	private void Start() {
		_UpdateCapEmission(transform.localPosition.y);
	}

	private void Update() {
		if (_changingLevel != LevelChange.NotChanging) {
			Vector3 pos = transform.localPosition;
			pos.y += TargetVerticalVelocity * Time.deltaTime;
			bool
				aboveTarget = pos.y > _targetHeight,
				movingUp = _changingLevel == LevelChange.Upwards;
			if (aboveTarget == movingUp) {
				// stop & snap to position
				pos.y = _targetHeight;
				_changingLevel = LevelChange.NotChanging;
			}
			transform.localPosition = pos;
			_UpdateCapEmission(pos.y);
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
		_UpdateCapEmission(pos.y);
	}
}
