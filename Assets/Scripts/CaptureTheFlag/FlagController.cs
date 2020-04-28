using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FlagController : MonoBehaviourPunCallbacks {
	public List<SpawnPoint> Spawns = new List<SpawnPoint>();

	/// <summary>
	/// The delay after the flag has been captured before another flag will be spawned.
	/// </summary>
	public float SpawnDelay = 3.0f;
	/// <summary>
	/// Spawn delay for the first flag.
	/// </summary>
	public float FirstSpawnDelay = 0.0f;

	private float _countdown = 0.0f;

	private GameObject _currentFlag;

	private PhotonView _network;
	private Teams _teams;

	public void Start() {
		_network = GetComponent<PhotonView>();
		_teams = GetComponent<Teams>();
		_countdown = FirstSpawnDelay;
	}

	protected virtual SpawnPoint _PickSpawn() {
		return Spawns[Random.Range(0, Spawns.Count)];
	}

	public void Update() {
		if (_network.IsMine) {
			if (!_currentFlag) {
				if (_countdown < 0.0f) {
					SpawnPoint spawn = _PickSpawn();
					Vector3? pos = spawn.GetSpawnLocation();
					if (pos.HasValue) {
						_currentFlag = PhotonNetwork.InstantiateSceneObject(
							Path.Combine("CaptureTheFlag", "Flag"), pos.Value, Quaternion.identity
						);
					}
				} else {
					_countdown -= Time.deltaTime;
				}
			} else {
				_countdown = SpawnDelay;
			}
		}
	}

	public override void OnMasterClientSwitched(Player newMasterClient) {
		base.OnMasterClientSwitched(newMasterClient);
		if (newMasterClient.IsLocal) {
			Debug.Assert(_network.IsMine);
			Flag flag = FindObjectOfType<Flag>();
			if (flag) {
				_currentFlag = flag.gameObject;
			}
		}
	}

	[PunRPC]
	public void RPC_OnFlagCaptured(int team, int flagViewId) {
		PhotonView flagView = PhotonView.Find(flagViewId);
		flagView.GetComponent<Animator>().SetTrigger("Captured");
		_teams.OnScore(team);
		Destroy(flagView.GetComponent<Flag>());
		Destroy(flagView.gameObject, 1.0f); // TODO magic number
		_currentFlag = null;
	}
}
