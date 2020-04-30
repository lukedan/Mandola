using Photon.Pun;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public enum TerrainAlterationType {
	Radial = 0,
	Wall = 1,
	None = 255
}

public class PrismSpawner : MonoBehaviour {
	public struct TerrainModification {
		public TerrainAlterationType Type;
		public Vector2 Position;
		public Vector2 Forward;
		public float RadiusOrLength;
		public float Delta;
	}

	/// <summary>
	/// The prism prefab.
	/// </summary>
	public GameObject PrismObject;
	/// <summary>
	/// The scale applied to prisms.
	/// </summary>
	public float PrismScale = 1.0f;
	/// <summary>
	/// The number of prisms on the X axis.
	/// </summary>
	public int NumPrismsX;
	/// <summary>
	/// The number of prisms on the Z axis.
	/// </summary>
	public int NumPrismsZ;
	/// <summary>
	/// Maximum height of all prisms.
	/// </summary>
	public float MaxHeight = 4.0f;
	/// <summary>
	/// Minimum height of all prisms.
	/// </summary>
	public float MinHeight = -4.0f;

	public Color MaxHeightColor = new Color(1.0f, 0.0f, 0.0f);
	public Color MinHeightColor = new Color(0.0f, 1.0f, 0.0f);

	/// <summary>
	/// The name of the map.
	/// </summary>
	public string MapName;

	public bool Ready { get; private set; } = false;

	public GameObject PlayerSpawnPrefab, FlagSpawnPrefab, FlagZonePrefab;

	/// <summary>
	/// The spacing between consecutive rows of prisms.
	/// </summary>
	private static readonly float HalfSqrt3 = 0.5f * Mathf.Sqrt(3.0f);

	private List<TerrainModification> _cachedTerrainModifications = new List<TerrainModification>();

	private void _PerformTerrainModification(TerrainModification mod, bool immediate) {
		Collider[] cols = null;
		switch (mod.Type) {
			case TerrainAlterationType.Radial:
				cols = Utils.GetPrismsInCylinder(mod.Position, mod.RadiusOrLength);
				break;
			case TerrainAlterationType.Wall:
				cols = Utils.GetPrismsInWall(mod.Position, mod.Forward, mod.RadiusOrLength);
				break;
		}
		Utils.AlterTerrain(cols, mod.Delta, immediate);
	}
	public void TryPerformTerrainModification(TerrainModification mod) {
		if (Ready) {
			_PerformTerrainModification(mod, false);
		} else {
			_cachedTerrainModifications.Add(mod);
		}
	}

	private T _Take<T>(IEnumerator<T> enumerator) {
		enumerator.MoveNext();
		return enumerator.Current;
	}
	private List<List<float>> _ParseMap(IEnumerator<string> se) {
		InGameCommon gameController = InGameCommon.CurrentGame;
		Teams teams = gameController.GetComponent<Teams>();

		// game mode
		//   0 - capture the flag
		int gameMode = int.Parse(_Take(se));
		// number of teams - n
		int numTeams = int.Parse(_Take(se));
		// n team colors
		{
			List<Color> colors = new List<Color>(numTeams);
			for (int i = 0; i < numTeams; ++i) {
				Color color = new Color();
				color.r = float.Parse(_Take(se));
				color.g = float.Parse(_Take(se));
				color.b = float.Parse(_Take(se));
				color.a = 1.0f;
				colors.Add(color);
			}
			teams.Colors = colors;
		}
		// n team spawn spefications
		{
			List<InGameCommon.SpawnList> spawns = new List<InGameCommon.SpawnList>(numTeams);
			for (int i = 0; i < numTeams; ++i) {
				int numSpawns = int.Parse(_Take(se));
				InGameCommon.SpawnList teamSpawns = new InGameCommon.SpawnList {
					Spawns = new List<SpawnPoint>(numSpawns)
				};
				for (int j = 0; j < numSpawns; ++j) {
					float x = float.Parse(_Take(se)) * PrismScale;
					float y = float.Parse(_Take(se)) * PrismScale;
					GameObject spawn = Instantiate(
						PlayerSpawnPrefab, new Vector3(x, 0.0f, y), Quaternion.identity
					);
					teamSpawns.Spawns.Add(spawn.GetComponent<SpawnPoint>());
				}
				spawns.Add(teamSpawns);
			}
			gameController.TeamSpawns = spawns;
		}
		if (gameMode == 0) {
			FlagController flagController = gameController.GetComponent<FlagController>();
			// flag zones
			for (int i = 0; i < numTeams; ++i) {
				int numFlagZones = int.Parse(_Take(se));
				for (int j = 0; j < numFlagZones; ++j) {
					float x = float.Parse(_Take(se)) * PrismScale;
					float y = float.Parse(_Take(se)) * PrismScale;
					float radius = float.Parse(_Take(se));
					Transform t =
						Instantiate(FlagZonePrefab, new Vector3(x, 0.0f, y), Quaternion.identity).transform;
					t.localScale = new Vector3(radius, 100.0f, radius);
					t.GetComponent<FlagZone>().Team = i;
				}
			}
			// flag spawns
			int numFlagSpawns = int.Parse(_Take(se));
			{
				List<SpawnPoint> spawns = new List<SpawnPoint>(numFlagSpawns);
				for (int i = 0; i < numFlagSpawns; ++i) {
					float x = float.Parse(_Take(se)) * PrismScale;
					float y = float.Parse(_Take(se)) * PrismScale;
					GameObject spawn = Instantiate(
						FlagSpawnPrefab, new Vector3(x, 0.0f, y), Quaternion.identity
					);
					spawns.Add(spawn.GetComponent<SpawnPoint>());
				}
				flagController.Spawns = spawns;
			}
		}
		// map properties
		NumPrismsX = int.Parse(_Take(se));
		NumPrismsZ = int.Parse(_Take(se));
		MinHeight = float.Parse(_Take(se));
		MaxHeight = float.Parse(_Take(se));
		// initial tile heights
		List<List<float>> heights = new List<List<float>>(NumPrismsZ);
		for (int z = 0; z < NumPrismsZ; ++z) {
			List<float> row = new List<float>(NumPrismsX);
			for (int x = 0; x < NumPrismsX; ++x) {
				row.Add(float.Parse(_Take(se)));
			}
			heights.Add(row);
		}
		return heights;
	}

	private void Awake() {
		// load map
		if (MapName.Length == 0) {
			MapName = (string)PhotonNetwork.CurrentRoom.CustomProperties[PhotonLobby.MapNamePropertyName];
		}
		TextAsset mapContents = Resources.Load<TextAsset>(Path.Combine("Maps", MapName));
		string[] strs = mapContents.text.Split(
			new char[] { ' ', '\t', '\n', '\r', ',', ';', '|' },
			StringSplitOptions.RemoveEmptyEntries
		);
		List<List<float>> heights = _ParseMap(((IEnumerable<string>)strs).GetEnumerator());

		bool flip = false;
		for (int x = 0; x < NumPrismsX; ++x) {
			bool innerFlip = flip;
			for (int z = 0; z < NumPrismsZ; ++z) {
				GameObject prism = Instantiate(PrismObject);

				Transform t = prism.transform;
				t.localPosition = new Vector3(HalfSqrt3 * x * PrismScale, 0.0f, 0.5f * z * PrismScale);
				if (innerFlip) {
					t.localEulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
				}
				t.localScale = new Vector3(PrismScale, t.localScale.y, PrismScale);

				PrismMover mover = prism.GetComponent<PrismMover>();
				mover.MaxHeight = MaxHeight;
				mover.MinHeight = MinHeight;
				mover.MinHeightCapColor = MinHeightColor;
				mover.MaxHeightCapColor = MaxHeightColor;
				mover.ChangeHeightImmediate(heights[z][x]);

				innerFlip = !innerFlip;
			}
			flip = !flip;
		}
		StartCoroutine(_WaitForPrismReady());
	}

	private IEnumerator _WaitForPrismReady() {
		// very hacky way to ensure that all prisms are spawned and ready
		yield return new WaitForFixedUpdate();

		Ready = true;
		foreach (TerrainModification mod in _cachedTerrainModifications) {
			_PerformTerrainModification(mod, true);
		}
		_cachedTerrainModifications = null;
	}
}
