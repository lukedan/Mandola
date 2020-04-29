using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamScoreCanvas : MonoBehaviour {
	public Color TeamColor;
	public int TeamIndex;

	public Text ScoreText;
	public Image ScoreImage;

	private Teams _teams;

	private int _Score => TeamIndex < _teams.Scores.Count ? _teams.Scores[TeamIndex] : 0;

	// Start is called before the first frame update
	void Start() {
		_teams = InGameCommon.CurrentGame.GetComponent<Teams>();
		ScoreImage.color = TeamColor;
		ScoreText.text = _Score.ToString();
	}

	// Update is called once per frame
	void Update() {
		ScoreText.text = _Score.ToString();
	}
}
