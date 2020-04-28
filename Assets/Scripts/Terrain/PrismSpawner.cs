using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TerrainAlterationType {
	Radial = 0,
	Wall = 1,
	None = 255
}

public class PrismSpawner : MonoBehaviour {
	public struct TerrainModification {
		public TerrainAlterationType Type;
		public Vector2 Position;
		public Vector2 Forward;
		public float RadiusOrLength;
		public float Delta;
	}

	/// <summary>
	/// The prism prefab.
	/// </summary>
	public GameObject PrismObject;
	/// <summary>
	/// The scale applied to prisms.
	/// </summary>
	public float PrismScale = 3.0f;
	/// <summary>
	/// The number of prisms on the X axis.
	/// </summary>
	public int NumPrismsX;
	/// <summary>
	/// The number of prisms on the Z axis.
	/// </summary>
	public int NumPrismsZ;
	/// <summary>
	/// Maximum height of all prisms.
	/// </summary>
	public float MaxHeight = 4.0f;
	/// <summary>
	/// Minimum height of all prisms.
	/// </summary>
	public float MinHeight = -4.0f;

	public bool Ready { get; private set; } = false;

	/// <summary>
	/// The spacing between consecutive rows of prisms.
	/// </summary>
	private static readonly float HalfSqrt3 = 0.5f * Mathf.Sqrt(3.0f);

	private List<TerrainModification> _cachedTerrainModifications = new List<TerrainModification>();

	private void _PerformTerrainModification(TerrainModification mod, bool immediate) {
		Collider[] cols = null;
		switch (mod.Type) {
			case TerrainAlterationType.Radial:
				cols = Utils.GetPrismsInCylinder(mod.Position, mod.RadiusOrLength);
				break;
			case TerrainAlterationType.Wall:
				cols = Utils.GetPrismsInWall(mod.Position, mod.Forward, mod.RadiusOrLength);
				break;
		}
		Utils.AlterTerrain(cols, mod.Delta, immediate);
	}
	public void TryPerformTerrainModification(TerrainModification mod) {
		if (Ready) {
			_PerformTerrainModification(mod, false);
		} else {
			_cachedTerrainModifications.Add(mod);
		}
	}

	private void Awake() {
		bool flip = false;
		for (int x = 0; x < NumPrismsX; ++x) {
			bool innerFlip = flip;
			for (int z = 0; z < NumPrismsZ; ++z) {
				GameObject prism = Instantiate(PrismObject);
				PrismMover mover = prism.GetComponent<PrismMover>();
				mover.MaxHeight = MaxHeight;
				mover.MinHeight = MinHeight;
				Transform t = prism.transform;
				t.localPosition = new Vector3(HalfSqrt3 * x * PrismScale, 0.0f, 0.5f * z * PrismScale);
				if (innerFlip) {
					t.localEulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
				}
				t.localScale = new Vector3(PrismScale, t.localScale.y, PrismScale);

				innerFlip = !innerFlip;
			}
			flip = !flip;
		}
		StartCoroutine(_WaitForPrismReady());
	}

	private IEnumerator _WaitForPrismReady() {
		// very hacky way to ensure that all prisms are spawned and ready
		yield return new WaitForFixedUpdate();

		Ready = true;
		foreach (TerrainModification mod in _cachedTerrainModifications) {
			_PerformTerrainModification(mod, true);
		}
		_cachedTerrainModifications = null;
	}
}
