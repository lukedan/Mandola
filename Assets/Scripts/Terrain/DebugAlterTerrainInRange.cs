using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugAlterTerrainInRange : MonoBehaviour {
	private PhotonView _gameNetwork;

	private void Start() {
		_gameNetwork = InGameCommon.CurrentGame.GetComponent<PhotonView>();
	}

	void Update() {
		if (Input.GetButtonDown("Ability")) {
			Physics.Raycast(
				Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit,
				float.PositiveInfinity, 1 << Utils.TerrainLayer
			);
			if (hit.collider != null) {
				_gameNetwork.RPC(
					"RPC_AlterTerrain", RpcTarget.AllBufferedViaServer,
					new Vector2(hit.point.x, hit.point.z), 5.0f, Input.GetAxisRaw("Ability")
				);
			}
		}
	}
}
