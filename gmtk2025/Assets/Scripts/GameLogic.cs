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
    public float gameTime = 0f;
    public float gameTickSpeed = 0.1f;
    float tickCounter = 0f;
    public float yearLength = 3.75f;
    float yearCounter = 0f;
    int year = 0;
    public int energy = 0;
    public int maxEnergy = 20;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ageText.text = "Age: 0";
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
            ageText.text = "Age: " + year;
        }

    }

    void GameTick()
    {
        return;
    }

    public void SliderChanged()
    {
        energy = maxEnergy - (int)(work.value + hobbies.value + social.value + exercise.value);
        energyText.text = "Energy: " + energy + "/" + maxEnergy;
    }

}
