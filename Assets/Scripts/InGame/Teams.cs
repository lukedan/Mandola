using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Teams : MonoBehaviour {
	public List<Color> Colors = new List<Color>();
	public List<int> Scores = new List<int>();

	/// <summary>
	/// Called when a team scores.
	/// </summary>
	/// <param name="team">The team that scored.</param>
	/// <param name="points">The number of points scored.</param>
	public void OnScore(int team, int points = 1) {
		while (Scores.Count <= team) {
			Scores.Add(0);
		}
		Scores[team] += points;
	}

	[PunRPC]
	public void RPC_OnScore(int team, int points) {
		OnScore(team, points);
	}
}
