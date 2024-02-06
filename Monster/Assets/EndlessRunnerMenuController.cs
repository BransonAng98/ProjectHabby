using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EndlessRunnerMenuController : MonoBehaviour
{
	public GameObject pauseMenu;
	public GameObject pauseBtn;

    public void StartGame()
    {
        SceneManager.LoadScene("Endless_Level_1");
    }

	public void PauseGame()
	{	
		pauseMenu.SetActive(true);
		pauseBtn.SetActive(false);
		Time.timeScale = 0;
	}

	public void ResumeGame()
	{
		pauseMenu.SetActive(false);
		pauseBtn.SetActive(true);
		Time.timeScale = 1;
	}

	public void RestartLevel()
	{
		string currentSceneName = SceneManager.GetActiveScene().name;
		SceneManager.LoadScene(currentSceneName);
	}

	public void ReturnToMainMenu()
    {
		SceneManager.LoadScene("EndlessRunnerMainMenu");
    }

	public void QuitGame()
    {
		Application.Quit();
    }
}
