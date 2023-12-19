using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Haptics.Vibrations;

public class MenuController : MonoBehaviour
{
	public MenuAMScript menuaudiomanager;
	public GameObject PauseMenu;
	public GameObject PauseButton;

    public void Start()
    {
		menuaudiomanager = GameObject.Find("MenuAudioManager").GetComponent<MenuAMScript>();
		VibrateHaptics.Initialize();
	}

    public void EnterGame()
	{
		menuaudiomanager.PlayTap();
		Debug.Log("Sound");
		SceneManager.LoadScene("LevelSelectScene");
	}

	public void PauseGame()
	{
		VibrateHaptics.VibrateClick();
		menuaudiomanager.PlayTap();
		PauseMenu.SetActive(true);
		PauseButton.SetActive(false);
		Time.timeScale = 0;
	}

	public void ResumeGame()
	{
		VibrateHaptics.VibrateClick();
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

	public void RestartLevel()
	{
		VibrateHaptics.VibrateClick();
		menuaudiomanager.PlayTap();
		string currentSceneName = SceneManager.GetActiveScene().name;
		StopVibration();
		SceneManager.LoadScene(currentSceneName);	
	}

	public void ToLobby()
    {
		VibrateHaptics.VibrateClick();
		menuaudiomanager.PlayTap();
		StopVibration();
		SceneManager.LoadScene("LevelSelectScene");
    }

	public void ToUpgrade()
    {
		SceneManager.LoadScene("UpgradeScene");
    }

	void StopVibration()
    {
		VibrateHaptics.Release();
    }
}
