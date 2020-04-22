using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugAlterTerrainInRange : MonoBehaviour {
	private PhotonView _network;

	private void Start() {
		_network = GetComponent<PhotonView>();
	}

	void Update() {
		if (Input.GetButtonDown("Ability")) {
			Physics.Raycast(
				Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit,
				float.PositiveInfinity, 1 << Utils.TerrainLayer
			);
			if (hit.collider != null) {
				_network.RPC(
					"RPC_AlterTerrain", RpcTarget.AllBufferedViaServer,
					new Vector2(hit.point.x, hit.point.z), 5.0f, Input.GetAxisRaw("Ability")
				);
			}
		}
	}

	[PunRPC]
	void RPC_AlterTerrain(Vector2 v, float radius, float delta) {
		Utils.AlterTerrainInCylinder(v, radius, delta);
	}
}
