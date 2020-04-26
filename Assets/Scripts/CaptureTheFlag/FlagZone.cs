using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagZone : MonoBehaviour {
	/// <summary>
	/// The team that owns this zone.
	/// </summary>
	public int Team = 0;

	private void Start() {
		Teams level = InGameCommon.CurrentGame.GetComponent<Teams>();
		List<Color> colors = level.Colors;
		if (Team < colors.Count) {
			GetComponent<MeshRenderer>().material.color = colors[Team];
		}
	}
}
