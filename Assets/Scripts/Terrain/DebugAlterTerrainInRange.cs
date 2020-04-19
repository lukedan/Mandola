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
				bool change = Input.GetKey(KeyCode.LeftControl);
				//Utils.AlterTerrainInCylinder(new Vector2(hit.point.x, hit.point.z), 5.0f, change ? -1 : 1);
				_network.RPC("RPC_AlterTerrain", RpcTarget.AllBuffered, new Vector2(hit.point.x, hit.point.z), 5.0f, change);
			}
		}
	}

	[PunRPC]
	void RPC_AlterTerrain(Vector2 v, float y, bool change) {
		Utils.AlterTerrainInCylinder(v, y, change ? -1 : 1);
	}
}
