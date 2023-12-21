using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class lmScoreDisplayScript : MonoBehaviour
{
   
    public TextMeshProUGUI goldText;
    public ResourceScriptableObject goldData;
    ///public AudioManagerScript audiomanager;
    [SerializeField] private int goldamt;
    private float lerpDuration = 2.0f;
    public lmScoreManagerScript scoreManager;
  
    public bool isWin;
    //public string formattedTime;
    private void Start()
    {
        scoreManager = GameObject.Find("ScoreManager").GetComponent<lmScoreManagerScript>();
        //audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();
        SetActiveScreen();
        StartCoroutine(LerpScores());
        goldamt = scoreManager.goldearned;
        goldData.currentGold += goldamt;
        //audiomanager.PlayPointCalculation();
    }

    private IEnumerator LerpScores()
    {
        float elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            float t = elapsedTime / lerpDuration;

            int gemsScore = Mathf.RoundToInt(Mathf.Lerp(0, scoreManager.goldearned, t));


            UpdateScoreUI(gemsScore);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final scores are set correctly
        UpdateScoreUI(scoreManager.goldearned);
    }

    private void UpdateScoreUI(int gems)
    {
        goldText.text = "" + gems;

    }

    public void SetActiveScreen()
    {
        isWin = true;
        goldText = GameObject.Find("winText_gems").GetComponent<TextMeshProUGUI>();
    }
}