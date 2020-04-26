using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FlagController : MonoBehaviour {
	public List<Transform> Spawns = new List<Transform>();

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

	protected virtual Transform _PickSpawn() {
		return Spawns[Random.Range(0, Spawns.Count)];
	}

	public void Update() {
		if (_network.IsMine) {
			if (!_currentFlag) {
				if (_countdown < 0.0f) {
					Transform spawn = _PickSpawn();
					_currentFlag = PhotonNetwork.InstantiateSceneObject(
						Path.Combine("CaptureTheFlag", "Flag"), spawn.position, spawn.rotation
					);
				} else {
					_countdown -= Time.deltaTime;
				}
			} else {
				_countdown = SpawnDelay;
			}
		}
	}

	[PunRPC]
	public void RPC_OnFlagCaptured(int team) {
		_currentFlag.GetComponent<Animator>().SetTrigger("Captured");
		_teams.OnScore(team);
		Destroy(_currentFlag, 1.0f);
		_currentFlag = null;
	}
}
