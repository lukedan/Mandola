using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {
	public GameObject MainMenu;
	public GameObject HelpMenu;

	// Start to play the game in opposing mode
	public void PlayOppsingMode() {
		SceneManager.LoadScene("NetworkScene");
	}

	public void ShowHelp() {
		MainMenu.SetActive(false);
		HelpMenu.SetActive(true);
	}

	public void HideHelp() {
		MainMenu.SetActive(true);
		HelpMenu.SetActive(false);
	}

	// Quit the game
	public void QuitGame() {
		Application.Quit();
	}
}
