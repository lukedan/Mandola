using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSelectionButton : MonoBehaviour {
	public Color TeamColor;
	public int TeamIndex;

	public Image ButtonImage;
	public Text ButtonText;

	private void Start() {
		ButtonImage.color = TeamColor;
		ButtonText.text = string.Format("Team {0}", TeamIndex + 1);
	}
}
