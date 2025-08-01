using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    public GameObject pauseOverlay;
    bool isPaused = false;
    bool forcedPause = false;
    public TMP_Text ageText;
    public TMP_Text energyText;
    public Slider work;
    public Slider hobbies;
    public Slider social;
    public Slider exercise;
    public TMP_Text happinessText;
    public TMP_Text healthText;
    public TMP_Text moneyText;
    public TMP_Text meaningText;
    public float gameTime = 0f;
    public float gameTickSpeed = 0.1f;
    float tickCounter = 0f;
    public float yearLength = 3.75f;
    float yearCounter = 0f;
    int year = 0;
    public int energy = 0;
    public int maxEnergy = 20;
    public float happiness = 50f;
    public float health = 90f;
    public float money = 0f;
    public float meaning = 0f;
    public float cumWork, cumHobbies, cumSocial, cumExercise, cumHappiness, cumHealth, cumMoney, cumMeaning;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ageText.text = "AGE: 0";
        SliderChanged();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused || forcedPause)
        {
            if(!pauseOverlay.activeSelf) pauseOverlay.SetActive(true);
            return;
        }
        else
        {
            if(pauseOverlay.activeSelf) pauseOverlay.SetActive(false);
        }

        gameTime += Time.deltaTime;
        tickCounter += Time.deltaTime;
        yearCounter += Time.deltaTime;

        while (tickCounter >= gameTickSpeed)
        {
            tickCounter -= gameTickSpeed;
            GameTick();
        }

        while (yearCounter >= yearLength)
        {
            yearCounter -= yearLength;
            year++;
            ageText.text = "AGE: " + year;
            Debug.Log(cumWork+","+cumHobbies+","+cumSocial+","+cumExercise+","+cumHappiness+","+cumHealth+","+cumMoney+","+cumMeaning);
        }

    }

    void OnPause()
    {
        isPaused = !isPaused;
    }

    // SliderChanged is called when any slider is moved
    public void SliderChanged()
    {
        energy = maxEnergy - (int)(work.value + hobbies.value + social.value + exercise.value);
        energyText.text = "ENERGY: " + energy + "/" + maxEnergy;
    }

    // GameTick is called based on gameTickSpeed
    void GameTick()
    {
        GenericStatUpdate();
        UpdateStatText();
    }

    void UpdateCumulativeValues()
    {
        cumWork += work.value * gameTickSpeed * 0.01f;
        cumHobbies += hobbies.value * gameTickSpeed * 0.01f;
        cumSocial += social.value * gameTickSpeed * 0.01f;
        cumExercise += exercise.value * gameTickSpeed * 0.01f;
        cumHappiness += (happiness - 30) * gameTickSpeed * 0.01f;
        cumHealth += (health - 30) * gameTickSpeed * 0.01f;
        cumMoney += money * gameTickSpeed * 0.01f;
        cumMeaning += meaning * gameTickSpeed * 0.01f;
    }

    //StatUpdates change money etc
    void GenericStatUpdate()
    {
        happiness += (social.value - 4) * 0.05f + Mathf.Min(10 - work.value, 0) * 0.1f;
        health += (exercise.value - 2) * 0.025f;
        money += (work.value - 5) * 0.05f - hobbies.value * 0.025f;
        meaning += (hobbies.value) * 0.025f;
    }

    void UpdateStatText()
    {
        moneyText.text = "" + (int) money;
        healthText.text = "" + (int) health;
        happinessText.text = "" + (int) happiness;
        meaningText.text = "" + (int) meaning;
    }

}
