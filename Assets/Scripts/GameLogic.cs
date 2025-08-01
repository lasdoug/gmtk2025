using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    public TMP_Text ageText;
    public TMP_Text energyText;
    public Slider work;
    public Slider hobbies;
    public Slider social;
    public Slider exercise;
    public TMP_Text happinessText;
    public TMP_Text healthText;
    public TMP_Text moneyText;
    public TMP_Text satisfactionText;
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
    public float satisfaction = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ageText.text = "AGE: 0";
        SliderChanged();
    }

    // Update is called once per frame
    void Update()
    {
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
        }

    }

    public void SliderChanged()
    {
        energy = maxEnergy - (int)(work.value + hobbies.value + social.value + exercise.value);
        energyText.text = "ENERGY: " + energy + "/" + maxEnergy;
    }



    void GameTick()
    {
        GenericStatUpdate();
        UpdateCumulativeValues();
        UpdateStatText();
    }

    void GenericStatUpdate()
    {
        happiness += (social.value - 4) * 0.05f + Mathf.Min(10 - work.value, 0) * 0.1f;
        health += (exercise.value - 2) * 0.025f;
        money += (work.value - 5) * 0.05f - hobbies.value * 0.025f;
        satisfaction += (hobbies.value) * 0.025f;
    }

    void UpdateStatText()
    {
        moneyText.text = "" + (int) money;
        healthText.text = "" + (int) health;
        happinessText.text = "" + (int) happiness;
        satisfactionText.text = "" + (int) satisfaction;
    }

}
