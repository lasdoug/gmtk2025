using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Mono.Cecil.Cil;
using TMPro;
using Unity.Multiplayer.Center.Common;
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
    public Slider play;
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
    public int maxEnergy = 40;
    public float happiness = 50f;
    public float health = 90f;
    public float money = 0f;
    public float meaning = 0f;

    public float scaling = 0.1f;
    public float[] dropoffMultipliers = { 1, 1, 1, 1 };
    // HAPPINESS , HEALTH , MONEY , MEANING
    public float[] workMultipliers = { 1, -0.25f, 1, 1 };
    public float[] playMultipliers = { 1, 1, -1, 1 };
    public float[] socialMultipliers = { 1, -1, -1, 1 };
    public float[] exerciseMultipliers = { 0.5f, 1, -1, 1 };
    private float[][] multipliersArray;
    // Vector3(mean, variance, mode)
    // mode: 0 no penalty, 1 penalty
    public Vector3[] workWindows = { new Vector3(7, 4, 1), new Vector3(20, 15, 0), new Vector3(20, 19, 1), new Vector3(12, 3, 0) };
    public Vector3[] playWindows = { new Vector3(20, 20, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17, 3, 0) };
    public Vector3[] socialWindows = { new Vector3(20, 15, 1), new Vector3(20, 2, 0), new Vector3(20, 6, 0), new Vector3(20, 5, 0) };
    public Vector3[] exerciseWindows = { new Vector3(12, 4, 1), new Vector3(20, 18, 1), new Vector3(20, 5, 0), new Vector3(0, 0, 0) };
    private Vector3[][] windowsArray;
    public float[] flatReductions = { -0.1f, -0.1f, -0.1f, -0.1f };
    List<Func<float>> calculateHappiness = new();
    List<Func<float>> calculateHealth = new();
    List<Func<float>> calculateMoney = new();
    List<Func<float>> calculateMeaning = new();

    public enum options
    {
        workE = 0,
        playE = 1,
        socialE = 2,
        exerciseE = 3
    }

    public float cumWork, cumHobbies, cumSocial, cumExercise, cumHappiness, cumHealth, cumMoney, cumMeaning;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ageText.text = "AGE: 0";
        SliderChanged();
        multipliersArray = new float[][] { workMultipliers, playMultipliers, socialMultipliers, exerciseMultipliers };
        windowsArray = new Vector3[][] { workWindows, playWindows, socialWindows, exerciseWindows };

        calculateHappiness.Add(WorkHappinessGain);
        calculateHappiness.Add(PlayHappinessGain);
        calculateHappiness.Add(SocialHappinessGain);
        calculateHappiness.Add(ExerciseHappinessGain);
        calculateHappiness.Add(HappinessFlatReduction);

        calculateHealth.Add(WorkHealthGain);
        calculateHealth.Add(PlayHealthGain);
        calculateHealth.Add(SocialHealthGain);
        calculateHealth.Add(ExerciseHealthGain);
        calculateHealth.Add(HealthFlatReduction);

        calculateMoney.Add(WorkMoneyGain);
        calculateMoney.Add(PlayMoneyGain);
        calculateMoney.Add(SocialMoneyGain);
        calculateMoney.Add(ExerciseMoneyGain);
        calculateMoney.Add(MoneyFlatReduction);

        calculateMeaning.Add(WorkMeaningGain);
        calculateMeaning.Add(PlayMeaningGain);
        calculateMeaning.Add(SocialMeaningGain);
        calculateMeaning.Add(ExerciseMeaningGain);
        calculateMeaning.Add(MeaningFlatReduction);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused || forcedPause)
        {
            if (!pauseOverlay.activeSelf) pauseOverlay.SetActive(true);
            return;
        }
        else
        {
            if (pauseOverlay.activeSelf) pauseOverlay.SetActive(false);
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
            // Debug.Log(cumWork + "," + cumHobbies + "," + cumSocial + "," + cumExercise + "," + cumHappiness + "," + cumHealth + "," + cumMoney + "," + cumMeaning);
        }

    }

    void OnPause()
    {
        isPaused = !isPaused;
    }

    // SliderChanged is called when any slider is moved
    public void SliderChanged()
    {
        energy = maxEnergy - (int)(work.value + play.value + social.value + exercise.value);
        energyText.text = "ENERGY: " + energy + "/" + maxEnergy;
    }

    // GameTick is called based on gameTickSpeed
    void GameTick()
    {
        StatUpdate();
        UpdateCumulativeValues();
        UpdateStatText();
    }

    void UpdateCumulativeValues()
    {
        cumWork += work.value * gameTickSpeed * 0.01f;
        cumHobbies += play.value * gameTickSpeed * 0.01f;
        cumSocial += social.value * gameTickSpeed * 0.01f;
        cumExercise += exercise.value * gameTickSpeed * 0.01f;
        cumHappiness += (happiness - 30) * gameTickSpeed * 0.01f;
        cumHealth += (health - 30) * gameTickSpeed * 0.01f;
        cumMoney += money * gameTickSpeed * 0.01f;
        cumMeaning += meaning * gameTickSpeed * 0.01f;
    }

    //StatUpdates change money etc
    void StatUpdate()
    {
        float hapGain = 0, hltGain = 0, monGain = 0, mngGain = 0;
        foreach (var func in calculateHappiness)
        {
            hapGain += func.Invoke();
        }
        foreach (var func in calculateHealth)
        {
            hltGain += func.Invoke();
        }
        foreach (var func in calculateMoney)
        {
            monGain += func.Invoke();
        }
        foreach (var func in calculateMeaning)
        {
            mngGain += func.Invoke();
        }
        happiness += hapGain;
        health += hltGain;
        money += monGain;
        meaning += mngGain;
        // Debug.Log(hapGain);
    }

    void UpdateStatText()
    {
        moneyText.text = "" + (int)money;
        healthText.text = "" + (int)health;
        happinessText.text = "" + (int)happiness;
        meaningText.text = "" + (int)meaning;
    }

    float WorkHappinessGain()
    {
        float gain = 0;
        gain += workWindows[0].y - Mathf.Abs(work.value - workWindows[0].x);
        if (gain < 0 && workWindows[0].z == 0) gain = 0;
        gain *= scaling * workMultipliers[0];
        return gain;
    }

    float PlayHappinessGain()
    {
        float gain = 0;
        gain += playWindows[0].y - Mathf.Abs(play.value - playWindows[0].x);
        if (gain < 0 && playWindows[0].z == 0) gain = 0;
        gain *= scaling * playMultipliers[0];
        return gain;
    }

    float AnyGain(int arrayIndex)
    {
        float gain = 0;
        gain += windowsArray[arrayIndex][0].y - Mathf.Abs(play.value - windowsArray[arrayIndex][0].x);
        if (gain < 0 && windowsArray[arrayIndex][0].z == 0) gain = 0;
        gain *= scaling * multipliersArray[arrayIndex][0];
        return gain;
    }

    float SocialHappinessGain()
    {
        float gain = 0;
        gain += socialWindows[0].y - Mathf.Abs(social.value - socialWindows[0].x);
        if (gain < 0 && socialWindows[0].z == 0) gain = 0;
        gain *= scaling * socialMultipliers[0];
        return gain;
    }

    float ExerciseHappinessGain()
    {
        float gain = 0;
        gain += exerciseWindows[0].y - Mathf.Abs(exercise.value - exerciseWindows[0].x);
        if (gain < 0 && exerciseWindows[0].z == 0) gain = 0;
        gain *= scaling * exerciseMultipliers[0];
        return gain;
    }

    float WorkHealthGain()
    {
        float gain = 0;
        gain += workWindows[1].y - Mathf.Abs(work.value - workWindows[1].x);
        if (gain < 0 && workWindows[1].z == 0) gain = 0;
        gain *= scaling * workMultipliers[1];
        return gain;
    }

    float PlayHealthGain()
    {
        float gain = 0;
        gain += playWindows[1].y - Mathf.Abs(play.value - playWindows[1].x);
        if (gain < 0 && playWindows[1].z == 0) gain = 0;
        gain *= scaling * playMultipliers[1];
        return gain;
    }

    float SocialHealthGain()
    {
        float gain = 0;
        gain += socialWindows[1].y - Mathf.Abs(social.value - socialWindows[1].x);
        if (gain < 0 && socialWindows[1].z == 0) gain = 0;
        gain *= scaling * socialMultipliers[1];
        return gain;
    }

    float ExerciseHealthGain()
    {
        float gain = 0;
        gain += exerciseWindows[1].y - Mathf.Abs(exercise.value - exerciseWindows[1].x);
        if (gain < 0 && exerciseWindows[1].z == 0) gain = 0;
        gain *= scaling * exerciseMultipliers[1];
        return gain;
    }

    float WorkMoneyGain()
    {
        float gain = 0;
        gain += workWindows[2].y - Mathf.Abs(work.value - workWindows[2].x);
        if (gain < 0 && workWindows[2].z == 0) gain = 0;
        gain *= scaling * workMultipliers[2];
        return gain;
    }

    float PlayMoneyGain()
    {
        float gain = 0;
        gain += playWindows[2].y - Mathf.Abs(play.value - playWindows[2].x);
        if (gain < 0 && playWindows[2].z == 0) gain = 0;
        gain *= scaling * playMultipliers[2];
        return gain;
    }

    float SocialMoneyGain()
    {
        float gain = 0;
        gain += socialWindows[2].y - Mathf.Abs(social.value - socialWindows[2].x);
        if (gain < 0 && socialWindows[2].z == 0) gain = 0;
        gain *= scaling * socialMultipliers[2];
        return gain;
    }

    float ExerciseMoneyGain()
    {
        float gain = 0;
        gain += exerciseWindows[2].y - Mathf.Abs(exercise.value - exerciseWindows[2].x);
        if (gain < 0 && exerciseWindows[2].z == 0) gain = 0;
        gain *= scaling * exerciseMultipliers[2];
        return gain;
    }

    float WorkMeaningGain()
    {
        float gain = 0;
        gain += workWindows[3].y - Mathf.Abs(work.value - workWindows[3].x);
        if (gain < 0 && workWindows[3].z == 0) gain = 0;
        gain *= scaling * workMultipliers[3];
        return gain;
    }

    float PlayMeaningGain()
    {
        float gain = 0;
        gain += playWindows[3].y - Mathf.Abs(play.value - playWindows[3].x);
        if (gain < 0 && playWindows[3].z == 0) gain = 0;
        gain *= scaling * playMultipliers[3];
        return gain;
    }

    float SocialMeaningGain()
    {
        float gain = 0;
        gain += socialWindows[3].y - Mathf.Abs(social.value - socialWindows[3].x);
        if (gain < 0 && socialWindows[3].z == 0) gain = 0;
        gain *= scaling * socialMultipliers[3];
        return gain;
    }

    float ExerciseMeaningGain()
    {
        float gain = 0;
        gain += exerciseWindows[3].y - Mathf.Abs(exercise.value - exerciseWindows[3].x);
        if (gain < 0 && exerciseWindows[3].z == 0) gain = 0;
        gain *= scaling * exerciseMultipliers[3];
        return gain;
    }

    float HappinessFlatReduction()
    {
        return flatReductions[0] * scaling;
    }

    float HealthFlatReduction()
    {
        return flatReductions[1] * scaling;
    }

    float MoneyFlatReduction()
    {
        return flatReductions[2] * scaling;
    }

    float MeaningFlatReduction()
    {
        return flatReductions[3] * scaling;
    }

}
