using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    public TMP_Text ageText;
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
    public float maxSliderValue = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ageText.text = "Age: 0";
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

    void ChangeValues(float cap, List<Slider> others)
    {
        float currentSum = 0f;
        foreach (Slider s in others)
        {
            currentSum += s.value;
        }

        foreach (Slider s in others)
        {
            s.value = s.value / currentSum * cap;
        }
    }

    public void WorkChanged()
    {
        if (work.value + hobbies.value + social.value + exercise.value > maxSliderValue)
        {
            ChangeValues(maxSliderValue - work.value, new List<Slider> { hobbies, exercise, social });
        }
    }

    public void HobbiesChanged()
    {
        if (work.value + hobbies.value + social.value + exercise.value > maxSliderValue)
        {
            ChangeValues(maxSliderValue - hobbies.value, new List<Slider> { work, exercise, social });
        }
    }

    public void SocialChanged()
    {
        if (work.value + hobbies.value + social.value + exercise.value > maxSliderValue)
        {
            ChangeValues(maxSliderValue - social.value, new List<Slider> { hobbies, exercise, work });
        }
    }

    public void ExerciseChanged()
    {
        if (work.value + hobbies.value + social.value + exercise.value > maxSliderValue)
        {
            ChangeValues(maxSliderValue - exercise.value, new List<Slider> { hobbies, work, social });
        }
    }
}
