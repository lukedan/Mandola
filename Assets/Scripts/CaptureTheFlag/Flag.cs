using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Flag : MonoBehaviour {
	private PhotonView _network;

	public float IndicatorShowBorder = 20.0f;
	public float IndicatorPositionBorder = 20.0f;
	public Texture IndicatorTexture;
	public Color IndicatorTint;

	private void Start() {
		_network = GetComponent<PhotonView>();
	}

	private void OnTriggerEnter(Collider other) {
		if (_network.IsMine) {
			if (!transform.parent) {
				if (other.gameObject.layer == Utils.PlayerLayer) {
					PhotonView collidingView = other.GetComponent<PhotonView>();
					if (!collidingView.IsMine) {
						_network.TransferOwnership(collidingView.Owner);
					}
					collidingView.RPC("RPC_OnPlayerGotFlag", RpcTarget.All, _network.ViewID);
				}
			}
			if (other.gameObject.layer == Utils.FlagZoneLayer) {
				InGameCommon.CurrentGame.GetComponent<PhotonView>().RPC(
					"RPC_OnFlagCaptured", RpcTarget.AllBufferedViaServer,
					other.GetComponent<FlagZone>().Team, _network.ViewID
				);
				Destroy(this);
			}
		}
	}

	private void OnGUI() {
		Vector3 pos = transform.position;
		Vector2 screenPos = Camera.main.WorldToViewportPoint(pos);
		if (Vector3.Dot(Camera.main.transform.forward, pos - Camera.main.transform.position) < 0.0f) {
			screenPos = new Vector2(0.5f, 0.5f) - screenPos;
		}
		screenPos.y = 1.0f - screenPos.y;
		Vector2 screenSize = new Vector2(Screen.width, Screen.height);
		screenPos *= screenSize;
		if (
			screenPos.x > IndicatorShowBorder && screenPos.x < screenSize.x - IndicatorShowBorder &&
			screenPos.y > IndicatorShowBorder && screenPos.y < screenSize.y - IndicatorShowBorder
		) {
			return;
		}
		// find intersection with screen borders
		Vector2 halfScreenSize = 0.5f * screenSize;
		Vector2 offset = screenPos - halfScreenSize;
		Vector2 target = halfScreenSize - new Vector2(IndicatorPositionBorder, IndicatorPositionBorder);
		if (offset.x < 0.0f) {
			target.x = -target.x;
		}
		if (offset.y < 0.0f) {
			target.y = -target.y;
		}
		Vector2 txy = target / offset;
		float t = Mathf.Min(txy.x, txy.y);
		Vector2 drawPos = offset * t + halfScreenSize;
		Vector2 textureSize = new Vector2(IndicatorTexture.width, IndicatorTexture.height);
		GUI.DrawTexture(
			new Rect(drawPos - 0.5f * textureSize, textureSize),
			IndicatorTexture, ScaleMode.ScaleToFit, true, 0.0f, IndicatorTint, 0.0f, 0.0f
		);
	}

	[PunRPC]
	public void RPC_OnPlayerKilled() {
		transform.parent = null;
		GetComponent<Rigidbody>().isKinematic = false;
	}
}
