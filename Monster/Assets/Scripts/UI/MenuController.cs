using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
	public MenuAMScript menuaudiomanager;
	public GameObject PauseMenu;
	public GameObject PauseButton;

    public void Start()
    {
		menuaudiomanager = GameObject.Find("MenuAudioManager").GetComponent<MenuAMScript>();
	}

    public void EnterGame()
	{
		menuaudiomanager.PlayTap();
		Debug.Log("Sound");
		SceneManager.LoadScene("LevelSelectScene");
	}

	public void PauseGame()
	{
		menuaudiomanager.PlayTap();
		PauseMenu.SetActive(true);
		PauseButton.SetActive(false);
		Time.timeScale = 0;
	}

	public void ResumeGame()
	{
		menuaudiomanager.PlayTap();
		PauseMenu.SetActive(false);
		PauseButton.SetActive(true);
		Time.timeScale = 1;

	}

	public void LoadLab()
    {
		menuaudiomanager.PlayTap();
		SceneManager.LoadScene("LabScene");
	}

	public void LoadGame()
	{
		menuaudiomanager.PlayTap();
		SceneManager.LoadScene("ComicScene");	
	}

	public void LeaveGame()
    {
		menuaudiomanager.PlayTap();
		SceneManager.LoadScene("LevelSelectScene");
    }
}
