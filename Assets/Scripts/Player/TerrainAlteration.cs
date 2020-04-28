using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainAlteration : MonoBehaviour {
	public readonly List<KeyValuePair<string, TerrainAlterationType>> AlterationButtons =
		new List<KeyValuePair<string, TerrainAlterationType>> {
			new KeyValuePair<string, TerrainAlterationType>("TerrainAlterationRadial", TerrainAlterationType.Radial),
			new KeyValuePair<string, TerrainAlterationType>("TerrainAlterationWall", TerrainAlterationType.Wall)
		};

	public float RadialAlterationRadius = 5.0f;
	public float RadialAlterationDelta = 1.0f;

	public float WallAlterationDistance = 5.0f;
	public float WallAlterationLength = 10.0f;
	public float WallAlterationDelta = 3.0f;

	public Mesh PreviewMesh;
	public Material PreviewMaterial;
	public Material PreviewNotReadyMaterial;
	public float PreviewVerticalOffset = 0.1f;

	/// <summary>
	/// Cooldown in seconds.
	/// </summary>
	public float Cooldown = 5.0f;

	private float _cooldownValue = 5.0f;

	private PlayerGeneralInfo _playerInfo;
	private PhotonView _gameNetwork;

	private void Start() {
		_playerInfo = GetComponent<PlayerGeneralInfo>();
		if (!GetComponent<PhotonView>().IsMine) {
			enabled = false;
			return;
		}
		_gameNetwork = InGameCommon.CurrentGame.GetComponent<PhotonView>();

		_cooldownValue = Cooldown;
	}

	private void Update() {
		if (!InGameCommon.CurrentGame.IsPaused) { // not paused
			TerrainAlterationType type = TerrainAlterationType.None;
			foreach (KeyValuePair<string, TerrainAlterationType> pair in AlterationButtons) {
				if (Input.GetButton(pair.Key)) {
					type = pair.Value;
					break;
				}
			}

			if (type != TerrainAlterationType.None) { // is holding down a button
				_playerInfo.ControlState = PlayerControlState.TerrainAlteration;
				if (Physics.Raycast(
					Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit,
					float.PositiveInfinity, 1 << Utils.TerrainLayer
				)) {
					Vector2 hitXZ = new Vector2(hit.point.x, hit.point.z);
					Vector2 pos = new Vector2(transform.position.x, transform.position.z);
					Vector2 diff = hitXZ - pos;
					if (type == TerrainAlterationType.Wall) {
						diff = diff.normalized;
						pos += diff * WallAlterationDistance;
					}
					if (Input.GetButton("Fire") && _cooldownValue < 0.0f) { // firing & can fire
						float multiplier = Input.GetAxisRaw("Fire");
						switch (type) {
							case TerrainAlterationType.Radial:
								_gameNetwork.RPC(
									"RPC_AlterTerrainRadial", RpcTarget.AllBufferedViaServer,
									hitXZ, RadialAlterationRadius, multiplier * RadialAlterationDelta
								);
								break;
							case TerrainAlterationType.Wall:
								_gameNetwork.RPC(
									"RPC_AlterTerrainWall", RpcTarget.AllBufferedViaServer,
									pos, diff, WallAlterationLength, multiplier * WallAlterationDelta
								);
								break;
						}
						_cooldownValue += Cooldown;
					} else { // show preview
						Collider[] cols = null;
						switch (type) {
							case TerrainAlterationType.Radial:
								cols = Utils.GetPrismsInCylinder(hitXZ, RadialAlterationRadius);
								break;
							case TerrainAlterationType.Wall:
								cols = Utils.GetPrismsInWall(pos, diff, WallAlterationLength);
								break;
						}
						Material mat = _cooldownValue < 0.0f ? PreviewMaterial : PreviewNotReadyMaterial;
						foreach (Collider col in cols) {
							Transform colTrans = col.transform;
							Vector3 previewPos = colTrans.position;
							previewPos.y += 0.5f * colTrans.localScale.y + PreviewVerticalOffset;
							Graphics.DrawMesh(
								PreviewMesh, Matrix4x4.TRS(previewPos, colTrans.localRotation, colTrans.localScale), mat,
								0, Camera.main, 0, null, false, false, false
							);
						}
					}
				}
			} else {
				_playerInfo.ControlState = PlayerControlState.Shooting;
			}
		}
		// cools down no matter paused or not
		_cooldownValue = Mathf.Max(_cooldownValue, 0.0f) - Time.deltaTime;
	}
}
