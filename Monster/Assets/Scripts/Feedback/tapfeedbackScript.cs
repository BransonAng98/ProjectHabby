using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tapfeedbackScript : MonoBehaviour
{
    public AudioSource feedbackaudioSource;
    public AudioClip[] feedbackSFX;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       /* for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                if (Input.touchCount == 1)
                {
                    PlaySFX();
                    Debug.Log("Touch");
                }
            }
        }*/
    }

    public void PlaySFX()
    {
        
    }
}
