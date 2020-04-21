using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {
	/// <summary>
	/// Layer mask for terrain.
	/// </summary>
	public static readonly int
		TerrainLayer = LayerMask.NameToLayer("Terrain"),
		PlayerLayer = LayerMask.NameToLayer("Player"),
		FlagLayer = LayerMask.NameToLayer("Flag");

	/// <summary>
	/// Alters all prisms in the specified range.
	/// </summary>
	/// <param name="center">The center position.</param>
	/// <param name="radius">The radius.</param>
	/// <param name="change">The change in levels.</param>
	public static void AlterTerrainInCylinder(Vector2 center, float radius, int change) {
		Collider[] cols = Physics.OverlapCapsule(
			new Vector3(center.x, -200.0f, center.y), new Vector3(center.x, 200.0f, center.y), radius,
			1 << TerrainLayer
		);
		foreach (Collider c in cols) {
			PrismMover mover = c.GetComponent<PrismMover>();
			if (mover != null) {
				mover.ChangeLevel(change);
			}
		}
	}
}
