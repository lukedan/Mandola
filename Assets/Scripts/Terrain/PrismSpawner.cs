using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrismSpawner : MonoBehaviour {
	/// <summary>
	/// The prism prefab.
	/// </summary>
	public GameObject PrismObject;
	/// <summary>
	/// The scale applied to prisms.
	/// </summary>
	public float PrismScale = 2.0f;
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

	/// <summary>
	/// The spacing between consecutive rows of prisms.
	/// </summary>
	private static readonly float HalfSqrt3 = 0.5f * Mathf.Sqrt(3.0f);

	private void Start() {
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
	}
}
