using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugAlterTerrainInRange : MonoBehaviour {
	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			Physics.Raycast(
				Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit,
				float.PositiveInfinity, ~Utils.TerrainLayerMask
			);
			if (hit.collider != null) {
				bool change = Input.GetKey(KeyCode.LeftControl);
				Utils.AlterTerrainInCylinder(new Vector2(hit.point.x, hit.point.z), 5.0f, change ? -1 : 1);
			}
		}
	}
}
