using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandmarkDesTutorial : MonoBehaviour
{
    public LevelManagerScriptableObject levelData;

    // Start is called before the first frame update
    void Start()
    {
        CheckForActivation();
    }

    void CheckForActivation()
    {
        if (levelData.landmarkTutorialPlayed)
        {
            this.gameObject.SetActive(false);
        }

        else
        {
            this.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!levelData.landmarkTutorialPlayed)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    PlayerPrefs.SetInt("LandmarkTutorial", 1);
                    levelData.landmarkTutorialPlayed = true;
                    PlayerPrefs.Save();
                    this.gameObject.SetActive(false);
                }
            }
        }

        else
        {
            return;
        }
    }
}
