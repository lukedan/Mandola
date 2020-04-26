using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class InGameCommon : MonoBehaviourPunCallbacks {
	public static InGameCommon CurrentGame;

	[System.Serializable]
	public class SpawnList {
		public List<Transform> Spawns;
	}

	public string SceneOnLeftRoom = "NetworkScene";
	/// <summary>
	/// Camera of the scene.
	/// </summary>
	public GameObject SceneCamera;
	/// <summary>
	/// My current player.
	/// </summary>
	public GameObject MyPlayer;
	/// <summary>
	/// Global post processing volume.
	/// </summary>
	public PostProcessVolume GlobalPostProcessing;
	/// <summary>
	/// Spawn points for each team.
	/// </summary>
	public List<SpawnList> TeamSpawns;

	private void Start() {
		CurrentGame = this;
		int playerTeam = 0;
		SpawnList list = TeamSpawns[playerTeam];
		Transform selectedSpawn = list.Spawns[Random.Range(0, list.Spawns.Count)];
		MyPlayer = PhotonNetwork.Instantiate(
			Path.Combine("PlayerPrefabs", "PlayerAvatar"), selectedSpawn.position, selectedSpawn.rotation, 0,
			new object[] { playerTeam, PlayerInfo.PI.mySelectedCharacter }
		);
	}

	public override void OnLeftRoom() {
		SceneManager.LoadScene(SceneOnLeftRoom);
	}

#if DEBUG
	private void OnGUI() {
		GUILayout.Label(PhotonNetwork.NetworkStatisticsToString());
	}
#endif
}
