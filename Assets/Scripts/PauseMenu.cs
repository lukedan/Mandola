using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	// A variable to determine whether game is paused or not
	public static bool IsPaused = false;

	public GameObject PausedMenuUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
        	if (IsPaused)
        	{
        		ResumeGame();
        	}
        	else
        	{
        		IsPaused = true;
        		PausedMenuUI.SetActive(true);
        	}
        }
    }

 	// Make the pause menu invisible to resume the game
    public void ResumeGame()
    {
    	IsPaused = false;
    	PausedMenuUI.SetActive(false);
    }

    // Return to the Lobby, i.e., the NetworkScene to join a new room
    public void BackToLobby()
    {
    	SceneManager.LoadScene("NetworkScene");
    }

    // Return to the start menu
    public void ExitGame()
    {
    	SceneManager.LoadScene("StartScene");
    }
}
