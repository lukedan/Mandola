using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItemDisplay : MonoBehaviour {
	/// <summary>
	/// The name of this room.
	/// </summary>
	public string Name;
	/// <summary>
	/// The number of players in the room.
	/// </summary>
	public int NumPlayers;
	/// <summary>
	/// The maximum number of players.
	/// </summary>
	public int MaxNumPlayers;

	public Text NameText;
	public Text NumPlayersText;

	private void Start() {
		NameText.text = Name;
		NumPlayersText.text = string.Format("{0} / {1}", NumPlayers, MaxNumPlayers);
	}
}
