using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {
	public const float InitialHeight = 10000.0f;

	/// <summary>
	/// The radius of the spawned object.
	/// </summary>
	public float Radius = 1.0f;

    /// <summary>
    /// Checks if spawning is available.
    /// </summary>
    /// <returns>The spawn location, or <code>null</code> if the terrain is not ready.</returns>
    public Vector3? GetSpawnLocation() {
		Vector3 xz = transform.position;
		if (Physics.SphereCast(
			new Vector3(xz.x, InitialHeight, xz.z), Radius, new Vector3(0.0f, -1.0f, 0.0f),
			out RaycastHit hit, float.MaxValue, 1 << Utils.TerrainLayer
		)) {
			return new Vector3(xz.x, hit.point.y + Radius, xz.z);
		}
		return null;
	}
}
