using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Teams : MonoBehaviour {
	public static Teams LevelTeams;

	public List<Color> Colors = new List<Color>();
	public List<int> Scores = new List<int>();

	private void Start() {
		LevelTeams = this;
	}

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
}
