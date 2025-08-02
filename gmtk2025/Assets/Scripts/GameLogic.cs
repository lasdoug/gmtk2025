using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using DG.Tweening;
using TMPro;
using Unity.Collections;
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
    public static int year = 0;
    public int energy = 0;
    public int maxEnergy = 40;
    public float happiness = 50f;
    public float health = 90f;
    public float money = 0f;
    public float meaning = 0f;
    bool energyTweening = false;
    DOTweenTMPAnimator animator;
    Sequence energyTextTween;
    NotifScript dialogueBox;

    public float scaling = 0.1f;
    public float[] dropoffMultipliers = { 1, 1, 1, 1 };
    // HAPPINESS , HEALTH , MONEY , MEANING
    float[] workMultipliers = { 0.5f, -0.3f, 0.4f, 0.1f };
    float[] playMultipliers = { 0.4f, 1, -1, 0.1f };
    float[] socialMultipliers = { 0.4f, -1, -1, 0.1f };
    float[] exerciseMultipliers = { 0.1f, 0.5f, -1, 0.1f };
    // Vector3(mean, variance, mode)
    // mode: 0 no penalty, 1 penalty
    Vector3[] workWindows = { new Vector3(6, 2, 1), new Vector3(20, 15, 0), new Vector3(20, 19, 1), new Vector3(12, 3, 0) };
    Vector3[] playWindows = { new Vector3(20, 20, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17, 3, 0) };
    Vector3[] socialWindows = { new Vector3(20, 15, 1), new Vector3(20, 2, 0), new Vector3(20, 6, 0), new Vector3(20, 5, 0) };
    Vector3[] exerciseWindows = { new Vector3(11, 6, 1), new Vector3(20, 18, 1), new Vector3(20, 5, 0), new Vector3(0, 0, 0) };
    float[] flatReductions = { -0.1f, -0.1f, -0.8f, -0.1f };
    float[] energyPenaltyMultipliers = { 1, 1, 1, 0.05f };

    List<Func<float>> calculateHappiness = new();
    List<Func<float>> calculateHealth = new();
    List<Func<float>> calculateMoney = new();
    List<Func<float>> calculateMeaning = new();

    public static float cumWork, cumHobbies, cumSocial, cumExercise, cumHappiness, cumHealth, cumMoney, cumMeaning;

    List<DialogueEvent> dialogueEvents = new();

    class DialogueEvent
    {
        GameLogic gameLogic;
        public float occurTime, upper;
        public float chance, work = 0, play = 0, social = 0, exercise = 0, happiness = 0, health = 0, money = 0, meaning = 0, happinesschange = 0, healthchange = 0, moneychange = 0, meaningchange = 0;
        public string message;
        public DialogueEvent(int lowerAge, int upperAge, float chanceOfOccurance)
        {
            occurTime = UnityEngine.Random.Range(lowerAge, upperAge);
            upper = upperAge;
            chance = chanceOfOccurance;
            gameLogic = FindAnyObjectByType<GameLogic>();
        }

        public void Check()
        {
            if (chance <= 0) return;
            if (GameLogic.year >= occurTime && GameLogic.year <= upper && AboveValues())
            {
                if (UnityEngine.Random.Range(0f, 1f) <= chance)
                {
                    gameLogic.Send(message);
                    gameLogic.happiness += happinesschange;
                    gameLogic.health += healthchange;
                    gameLogic.money += moneychange;
                    gameLogic.meaning += meaningchange;
                }
                chance = -1;
            }
        }

        bool AboveValues() {
            return GameLogic.cumWork >= work && GameLogic.cumHobbies >= play && GameLogic.cumSocial >= social && GameLogic.cumExercise >= exercise && GameLogic.cumHappiness >= happiness && GameLogic.cumHealth >= health && GameLogic.cumMoney >= money && GameLogic.cumMeaning >= meaning;
        }

        public void SetMessage(string str)
        {
            message = str;
        }

        public void SetWork(float x)
        {
            work = x;
        }
        public void SetPlay(float x)
        {
            play = x;
        }
        public void SetSocial(float x)
        {
            social = x;
        }
        public void SetExercise(float x)
        {
            exercise = x;
        }
        public void SetHappiness(float x)
        {
            happiness = x;
        }
        public void SetHealth(float x)
        {
            health = x;
        }
        public void SetMoney(float x)
        {
            money = x;
        }
        public void SetMeaning(float x)
        {
            meaning = x;
        }
        public void SetHappinessChange(float x)
        {
            happinesschange = x;
        }
        public void SetHealthChange(float x)
        {
            healthchange = x;
        }
        public void SetMoneyChange(float x)
        {
            moneychange = x;
        }
        public void SetMeaningChange(float x)
        {
            meaningchange = x;
        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueBox = FindAnyObjectByType<NotifScript>();
        Send("You are born. Hello World.");
        ageText.text = "AGE: 0";
        animator = new DOTweenTMPAnimator(energyText);
        SliderChanged();

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

        DialogueEvent newEvent;
        newEvent = new DialogueEvent(0, 100, 1);
        newEvent.SetExercise(0.35f);
        newEvent.SetMessage("You learned to walk.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(0, 100, 1);
        newEvent.SetMessage("You learned to talk.");
        newEvent.SetSocial(0.4f);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(7, 14, 0.25f);
        newEvent.SetMessage("You found £20 on the floor.");
        newEvent.SetMoneyChange(3f);
        dialogueEvents.Add(newEvent);

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
            Debug.Log("Cumulative values: "+cumWork + "," + cumHobbies + "," + cumSocial + "," + cumExercise + "," + cumHappiness + "," + cumHealth + "," + cumMoney + "," + cumMeaning);
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
        if (energy < 0 && !energyTweening)
        {
            TweenText();
            energyTweening = true;
        }
        else if (energy >= 0 && energyTweening)
        {
            energyTextTween.Kill();
            energyTweening = false;
        }
    }

    // GameTick is called based on gameTickSpeed
    void GameTick()
    {
        StatUpdate();
        UpdateCumulativeValues();
        UpdateStatText();
        DoChecks();
    }

    void Send(string str)
    {
        dialogueBox.QueueMessage(str);
    }

    void UpdateCumulativeValues()
    {
        cumWork += work.value * gameTickSpeed / scaling * 0.001f;
        cumHobbies += play.value * gameTickSpeed / scaling * 0.001f;
        cumSocial += social.value * gameTickSpeed / scaling * 0.001f;
        cumExercise += exercise.value * gameTickSpeed / scaling * 0.001f;
        cumHappiness += (happiness - 30) * gameTickSpeed / scaling * 0.001f;
        cumHealth += (health - 30) * gameTickSpeed / scaling * 0.001f;
        cumMoney += money * gameTickSpeed / scaling * 0.001f;
        cumMeaning += meaning * gameTickSpeed / scaling * 0.001f;
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
        happiness += Mathf.Max(happiness + hapGain + Mathf.Min((float)energy, 0f)*scaling*energyPenaltyMultipliers[0], 0);
        health += Mathf.Max(health + hltGain + Mathf.Min((float)energy, 0f)*scaling*energyPenaltyMultipliers[1], 0);
        money += Mathf.Max(money + monGain + Mathf.Min((float)energy, 0f)*scaling*energyPenaltyMultipliers[2], 0);
        meaning += Mathf.Max(meaning + mngGain + Mathf.Min((float)energy, 0f)*scaling*energyPenaltyMultipliers[3], 0);
        //Debug.Log(hapGain);
    }

    void UpdateStatText()
    {
        moneyText.text = "" + (int)money;
        healthText.text = "" + (int)health;
        happinessText.text = "" + (int)happiness;
        meaningText.text = "" + (int)meaning;
    }

    void TweenText()
    {
        energyTextTween = DOTween.Sequence();
        for (int i = 0; i < 6; ++i)
        {
            if (!animator.textInfo.characterInfo[i].isVisible) continue;
            energyTextTween.Join(animator.DOShakeCharOffset(i, 0.1f, 2f, 7, 45f));
        }
        energyTextTween.SetLoops(-1, LoopType.Restart);
    }

    void DoChecks()
    {
        foreach (DialogueEvent e in dialogueEvents)
        {
            e.Check();
        }
    }


    float WorkHappinessGain()
    {
        float gain = 0;
        gain += workWindows[0].y - Mathf.Abs(work.value - workWindows[0].x);
        if (gain < 0 && workWindows[0].z == 0) gain = 0;
        gain *= scaling * workMultipliers[0];
        //Debug.Log(gain);
        return gain;
    }

    float PlayHappinessGain()
    {
        float gain = 0;
        gain += playWindows[0].y - Mathf.Abs(play.value - playWindows[0].x);
        if (gain < 0 && playWindows[0].z == 0) gain = 0;
        gain *= scaling * playMultipliers[0];
        //Debug.Log(gain);
        return gain;
    }

    float SocialHappinessGain()
    {
        float gain = 0;
        gain += socialWindows[0].y - Mathf.Abs(social.value - socialWindows[0].x);
        if (gain < 0 && socialWindows[0].z == 0) gain = 0;
        gain *= scaling * socialMultipliers[0];
        //Debug.Log(gain);
        return gain;
    }

    float ExerciseHappinessGain()
    {
        float gain = 0;
        gain += exerciseWindows[0].y - Mathf.Abs(exercise.value - exerciseWindows[0].x);
        if (gain < 0 && exerciseWindows[0].z == 0) gain = 0;
        gain *= scaling * exerciseMultipliers[0];
        //Debug.Log(gain);
        return gain;
    }

    float WorkHealthGain()
    {
        float gain = 0;
        gain += workWindows[1].y - Mathf.Abs(work.value - workWindows[1].x);
        if (gain < 0 && workWindows[1].z == 0) gain = 0;
        gain *= scaling * workMultipliers[1];
        //Debug.Log(gain);
        return gain;
    }

    float PlayHealthGain()
    {
        float gain = 0;
        gain += playWindows[1].y - Mathf.Abs(play.value - playWindows[1].x);
        if (gain < 0 && playWindows[1].z == 0) gain = 0;
        gain *= scaling * playMultipliers[1];
        //Debug.Log(gain);
        return gain;
    }

    float SocialHealthGain()
    {
        float gain = 0;
        gain += socialWindows[1].y - Mathf.Abs(social.value - socialWindows[1].x);
        if (gain < 0 && socialWindows[1].z == 0) gain = 0;
        gain *= scaling * socialMultipliers[1];
        //Debug.Log(gain);
        return gain;
    }

    float ExerciseHealthGain()
    {
        float gain = 0;
        gain += exerciseWindows[1].y - Mathf.Abs(exercise.value - exerciseWindows[1].x);
        if (gain < 0 && exerciseWindows[1].z == 0) gain = 0;
        gain *= scaling * exerciseMultipliers[1];
        //Debug.Log(gain);
        return gain;
    }

    float WorkMoneyGain()
    {
        float gain = 0;
        gain += workWindows[2].y - Mathf.Abs(work.value - workWindows[2].x);
        if (gain < 0 && workWindows[2].z == 0) gain = 0;
        gain *= scaling * workMultipliers[2];
        //Debug.Log(gain);
        return gain;
    }

    float PlayMoneyGain()
    {
        float gain = 0;
        gain += playWindows[2].y - Mathf.Abs(play.value - playWindows[2].x);
        if (gain < 0 && playWindows[2].z == 0) gain = 0;
        gain *= scaling * playMultipliers[2];
        //Debug.Log(gain);
        return gain;
    }

    float SocialMoneyGain()
    {
        float gain = 0;
        gain += socialWindows[2].y - Mathf.Abs(social.value - socialWindows[2].x);
        if (gain < 0 && socialWindows[2].z == 0) gain = 0;
        gain *= scaling * socialMultipliers[2];
        //Debug.Log(gain);
        return gain;
    }

    float ExerciseMoneyGain()
    {
        float gain = 0;
        gain += exerciseWindows[2].y - Mathf.Abs(exercise.value - exerciseWindows[2].x);
        if (gain < 0 && exerciseWindows[2].z == 0) gain = 0;
        gain *= scaling * exerciseMultipliers[2];
        //Debug.Log(gain);
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

    //call this when starting another run
    //probably call other stuff too tbh
    void resetCumValues()
    {
        cumWork = 0;
        cumHobbies = 0;
        cumSocial = 0;
        cumExercise = 0;
        cumHappiness = 0;
        cumHealth = 0;
        cumMoney = 0;
        cumMeaning = 0;
    }

}
