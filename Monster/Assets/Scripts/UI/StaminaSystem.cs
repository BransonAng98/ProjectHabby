using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class StaminaSystem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI energyText;
    [SerializeField] TextMeshProUGUI timerText;

    public GameObject noEnergyPrefab;
    public Canvas canvas;

    private int maxEnergy = 25;
    public int currentEnergy;
    private int restoreDuration = 3;
    private DateTime nextEnergyTime;
    private DateTime lastEnergyTime;
    private bool isRestoring = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("currentEnergy"))
        {
            PlayerPrefs.SetInt("currentEnergy", 25);
            Load();
            UpdateEnergy();
            StartCoroutine(RestoreEnergy());
        }
        else
        {
            Load();
            UpdateEnergy();
            StartCoroutine(RestoreEnergy());
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            UseEnergy();
        }

          // Example: Manually reset the values when pressing R key
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetStamina();
        }
    }


    public void ResetStamina()
    {
        // Reset all stamina-related values to their initial state
        currentEnergy = maxEnergy;
        
        nextEnergyTime = AddDuration(DateTime.Now, restoreDuration);
        lastEnergyTime = DateTime.Now;

        // Reset the timer to the initial duration
        UpdateEnergy();
        UpdateEnergyTimer();

        // Save the reset values
        Save();

        // Restart the coroutine for energy restoration
        StartCoroutine(RestoreEnergy());
    }

    private void UseEnergy()
    { 
        if (currentEnergy >= 5)
        {
            currentEnergy -= 5;
            UpdateEnergy();
            if (isRestoring == false)
            {
                if (currentEnergy + 1 == maxEnergy)
                {
                    nextEnergyTime = AddDuration(DateTime.Now, restoreDuration);
                }
            }
            StartCoroutine(RestoreEnergy());
        }
        else
        {
            GameObject prompter = Instantiate(noEnergyPrefab);
            // Set the position of the instantiated object
            prompter.transform.SetParent(canvas.transform, false);
        }
    }


    private IEnumerator RestoreEnergy()
    {
        UpdateEnergyTimer();
        isRestoring = true;
        while (currentEnergy < maxEnergy)
        {
            DateTime currentDateTime = DateTime.Now;
            DateTime nextDateTime = nextEnergyTime;
            bool isEnergyAdding = false;

            while (currentDateTime > nextDateTime)
            {
                if (currentEnergy < maxEnergy)
                {
                    isEnergyAdding = true;
                    currentEnergy++;
                    UpdateEnergy();
                    DateTime timeToAdd = lastEnergyTime > nextDateTime ? lastEnergyTime : nextDateTime;
                    nextDateTime = AddDuration(timeToAdd, restoreDuration);
                }

                else
                {
                    break;
                }
            }

            if (isEnergyAdding == true)
            {
                lastEnergyTime = DateTime.Now;
                nextEnergyTime = nextDateTime;
            }

            UpdateEnergyTimer();
            UpdateEnergy();
            Save();
            yield return null;
        }

        isRestoring = false; 
    }
    
    private DateTime AddDuration(DateTime dateTime, int duration)
    {
        //return dateTime.AddSeconds(duration); for testing
        return dateTime.AddMinutes(duration);
    }

    private void UpdateEnergyTimer()
    {
        if (currentEnergy >= maxEnergy)
        {
            timerText.text = "Full";
            return;
        }

        TimeSpan time = nextEnergyTime - DateTime.Now;
        string timeValue = String.Format("{0:D2}:{1:D1}", time.Minutes, time.Seconds);
        timerText.text = timeValue;
    }

    private void UpdateEnergy()
    {
        energyText.text = currentEnergy.ToString() + "/" + maxEnergy.ToString();
    }


    private DateTime StringToDate(string dateTime)
    {
        if (String.IsNullOrEmpty(dateTime))
        {
            return DateTime.Now;
        }
        else
        {
            return DateTime.Parse(dateTime);
        }
    }

    private void Load()
    {
        currentEnergy = PlayerPrefs.GetInt("currentEnergy");
        nextEnergyTime = StringToDate(PlayerPrefs.GetString("nextEnergyTime"));
        lastEnergyTime = StringToDate(PlayerPrefs.GetString("lastEnergyTime"));
    }


    private void Save()
    {
        PlayerPrefs.SetInt("currentEnergy", currentEnergy);
        PlayerPrefs.SetString("nextEnergyTime", nextEnergyTime.ToString());
        PlayerPrefs.SetString("lastEnergyTime", lastEnergyTime.ToString());
    }
}
