using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneManager : MonoBehaviour
{
    public GameManagerScript gameManager;
    public Animator anim;
    private PlayerHandler inputHandler;
    public AudioManagerScript audiomanager;


    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManagerScript>();
        inputHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHandler>();
        anim = GetComponent<Animator>();
        audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
    }


    public void CallEndScreen()
    {
        gameManager.TriggerEndScreen();
    }

    public void TriggerEnd()
    {
        audiomanager.StopBGM();
        inputHandler.enableInput = false;
        anim.SetBool("isTriggered", true);
    }

    public void ResetTrigger()
    {
        anim.SetBool("isTriggered", false);
    }

}
