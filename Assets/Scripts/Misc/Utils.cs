﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {
	/// <summary>
	/// Layer mask for terrain.
	/// </summary>
	public static readonly int
		TerrainLayer = LayerMask.NameToLayer("Terrain"),
		PlayerLayer = LayerMask.NameToLayer("Player"),
		FlagLayer = LayerMask.NameToLayer("Flag"),
		FlagZoneLayer = LayerMask.NameToLayer("FlagZone");

	public static Collider[] GetPrismsInCylinder(Vector2 center, float radius) {
		return Physics.OverlapCapsule(
			new Vector3(center.x, -200.0f, center.y), new Vector3(center.x, 200.0f, center.y), radius,
			1 << TerrainLayer
		);
	}
	public static Collider[] GetPrismsInWall(Vector2 center, Vector2 forward, float length) {
		return Physics.OverlapBox(
			new Vector3(center.x, 0.0f, center.y), new Vector3(0.5f * length, 200.0f, 0.0f),
			Quaternion.FromToRotation(Vector3.forward, new Vector3(forward.x, 0.0f, forward.y)),
			1 << TerrainLayer
		);
	}

	/// <summary>
	/// Alters all given prisms.
	/// </summary>
	/// <param name="cols">The prisms.</param>
	/// <param name="delta">The change in height.</param>
	/// <param name="immediate">Whether this change is immediate.</param>
	public static void AlterTerrain(Collider[] cols, float delta, bool immediate) {
		foreach (Collider c in cols) {
			PrismMover mover = c.GetComponent<PrismMover>();
			if (mover != null) {
				if (immediate) {
					mover.ChangeHeightImmediate(delta);
				} else {
					mover.ChangeHeight(delta);
				}
			}
		}
	}
}
