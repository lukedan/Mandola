using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelection : MonoBehaviour {
	public GameObject TeamButtonPrefab;

	private void Start() {
		Teams teams = InGameCommon.CurrentGame.GetComponent<Teams>();
		for (int i = 0; i < teams.Colors.Count; ++i) {
			GameObject button = Instantiate(TeamButtonPrefab, transform);
			TeamSelectionButton logic = button.GetComponent<TeamSelectionButton>();
			logic.TeamColor = teams.Colors[i];
			logic.TeamIndex = i;
			button.GetComponent<Button>().onClick.AddListener(() => {
				gameObject.SetActive(false);
				InGameCommon.CurrentGame.PlayerTeam = logic.TeamIndex;
			});
		}
	}
}
