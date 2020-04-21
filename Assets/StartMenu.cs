using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    // Start to play the game in opposing mode
    public void PlayOppsingMode()
    {
        SceneManager.LoadScene("NetworkScene");
    }

    // Start to play the game in grouping mode
    public void PlayGroupingMode()
    {
    	// Change this to the "GroupModeScene" after implementing it.
        SceneManager.LoadScene("NetworkScene");
    }

    // Quit the game
    public void QuitGame()
    {
        Application.Quit();
    }
}
