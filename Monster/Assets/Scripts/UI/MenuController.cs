using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
	public AudioManagerScript audiomanager;
	public GameObject PauseMenu;
	public GameObject PauseButton;

    public void Start()
    {
		audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
	}

    public void EnterGame()
	{
		SceneManager.LoadScene("LevelSelectScene");
	}

	public void PauseGame()
	{
		
		PauseMenu.SetActive(true);
		PauseButton.SetActive(false);
		Time.timeScale = 0;
		audiomanager.PlayTap();
	}

	public void ResumeGame()
	{
		audiomanager.PlayTap();
		PauseMenu.SetActive(false);
		PauseButton.SetActive(true);
		Time.timeScale = 1;

	}

	public void LoadLab()
    {
		audiomanager.PlayTap();
		SceneManager.LoadScene("LabScene");
	}

	public void LoadGame()
	{
		audiomanager.PlayTap();
		SceneManager.LoadScene("ComicScene");	
	}

	public void LeaveGame()
    {
		audiomanager.PlayTap();
		SceneManager.LoadScene("LevelSelectScene");
    }
}
