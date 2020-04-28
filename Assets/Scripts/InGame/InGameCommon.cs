using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class InGameCommon : MonoBehaviourPunCallbacks {
	public static InGameCommon CurrentGame;

	[System.Serializable]
	public class SpawnList {
		public List<SpawnPoint> Spawns;
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
	/// Spawns the terrain.
	/// </summary>
	public PrismSpawner TerrainSpawner;
	/// <summary>
	/// Spawn points for each team.
	/// </summary>
	public List<SpawnList> TeamSpawns;

	/// <summary>
	/// Records avatars that will be destroyed when a player leaves the room.
	/// </summary>
	public Dictionary<int, GameObject> PlayerAvatars = new Dictionary<int, GameObject>();

	/// <summary>
	/// The team to spawn the player with.
	/// </summary>
	public int PlayerTeam = -1;
	/// <summary>
	/// How quickly the damage effect decays after death.
	/// </summary>
	public float DeadEffectDecay = 2.0f;

	private float _respawnDelay = -1.0f;

	private void Awake() {
		CurrentGame = this;
	}

	private void Update() {
		if (!MyPlayer) {
			if (PlayerTeam >= 0 && _respawnDelay < 0.0f) {
				SpawnList list = TeamSpawns[PlayerTeam];
				SpawnPoint selectedSpawn = list.Spawns[Random.Range(0, list.Spawns.Count)];
				Vector3? spawnPos = selectedSpawn.GetSpawnLocation();
				if (spawnPos.HasValue) {
					MyPlayer = PhotonNetwork.Instantiate(
						Path.Combine("PlayerPrefabs", "PlayerAvatar"), spawnPos.Value, Quaternion.identity, 0,
						new object[] { PlayerTeam, PlayerInfo.PI.mySelectedCharacter }
					);
				}
			} else {
				_respawnDelay -= Time.deltaTime;
				// visual effects
				ChromaticAberration settings;
				GlobalPostProcessing.profile.TryGetSettings(out settings);
				settings.intensity.Override(Mathf.Max(
					0.0f, settings.intensity.value - DeadEffectDecay * Time.deltaTime
				));
			}
		}
	}

	/// <summary>
	/// Resets <see cref="_respawnDelay"/> based on room settings.
	/// </summary>
	public void OnPlayerKilled() {
		_respawnDelay = (float)PhotonNetwork.CurrentRoom.CustomProperties[PhotonLobby.RespawnDelayPropertyName];
	}

	public override void OnPlayerLeftRoom(Player otherPlayer) {
		base.OnPlayerLeftRoom(otherPlayer);
		if (PhotonNetwork.IsMasterClient) {
			if (PlayerAvatars.TryGetValue(otherPlayer.ActorNumber, out GameObject avatar)) {
				PhotonNetwork.Destroy(avatar);
				PlayerAvatars.Remove(otherPlayer.ActorNumber);
			}
		}
	}
	public override void OnLeftRoom() {
		SceneManager.LoadScene(SceneOnLeftRoom);
	}


	[PunRPC]
	void RPC_AlterTerrain(Vector2 v, float radius, float delta) {
		if (TerrainSpawner.Ready) {
			Utils.AlterTerrainInCylinder(v, radius, delta, false);
		} else {
			TerrainSpawner.CachedTerrainModifications.Add(new PrismSpawner.TerrainModification {
				Position = v,
				Radius = radius,
				Delta = delta
			});
		}
	}


#if DEBUG
	private void OnGUI() {
		GUILayout.Label(PhotonNetwork.NetworkStatisticsToString());
	}
#endif
}
