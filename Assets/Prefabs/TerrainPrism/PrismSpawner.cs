using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrismSpawner : MonoBehaviour {
	/// <summary>
	/// The prism prefab.
	/// </summary>
	public GameObject PrismObject;
	/// <summary>
	/// The number of prisms on the X axis.
	/// </summary>
	public int NumPrismsX;
	/// <summary>
	/// The number of prisms on the Z axis.
	/// </summary>
	public int NumPrismsZ;

	/// <summary>
	/// The spacing between consecutive rows of prisms.
	/// </summary>
	private static readonly float HalfSqrt3 = 0.5f * Mathf.Sqrt(3.0f);

	private void Start() {
		bool flip = false;
		for (int x = 0; x < NumPrismsX; ++x) {
			bool innerFlip = flip;
			for (int z = 0; z < NumPrismsZ; ++z) {
				Transform t = Instantiate(PrismObject).transform;
				if (innerFlip) {
					t.localEulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
				}
				t.localPosition = new Vector3(HalfSqrt3 * x, 0.0f, 0.5f * z);

				innerFlip = !innerFlip;
			}
			flip = !flip;
		}
	}
}
