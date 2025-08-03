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
    public StatBlockScript happinessDisplay, healthDisplay, moneyDisplay, meaningDisplay;
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
    public float yearCounter = 0f;
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
    float[] exerciseMultipliers = { 0.1f, 0.4f, -1, 0.1f };
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
    string[] firstWords = { "THROAT", "RICE", "CAR", "NOODLES", "MOMMA", "DADDA", "CELERY", "FRESCO", "CONGLOMERATE",  "NO" };
    string [] hobbies = {"DINOSAURS", "TRUCKS", "THE STOCK MARKET", "MAGIC", "MAGIC THE GATHERING", "BUGS", "BASS GUITAR"};

    string [] personalProjects = {"A FILM ABOUT DUST", "AN INDIE GAME ABOUT FOXES", "AN EP OF HOUSE MUSIC", "A PAINTING OF YOUR HOUSE"};

    class DialogueEvent
    {
        GameLogic gameLogic;
        public float lower, upper;
        public float chance, work = 0, play = 0, social = 0, exercise = 0, happiness = 0, health = 0, money = 0, meaning = 0, happinesschange = 0, healthchange = 0, moneychange = 0, meaningchange = 0;
        public string message;
        public DialogueEvent(float lowerAge, float upperAge, float chanceOfOccurance)
        {
            lower = lowerAge + UnityEngine.Random.Range(0f, 0.5f);
            upper = upperAge + UnityEngine.Random.Range(-0.5f, 0.5f);
            chance = chanceOfOccurance;
            gameLogic = FindAnyObjectByType<GameLogic>();
        }

        public void Check()
        {
            if (chance <= 0) return;
            if (GameLogic.year + gameLogic.yearCounter / gameLogic.yearLength >= lower && GameLogic.year + gameLogic.yearCounter / gameLogic.yearLength <= upper && AboveValues())
            {
                Debug.Log(message);
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

        bool AboveValues()
        {
            bool ans = true;
            if (work < 0)
            {
                ans = ans && GameLogic.cumWork <= -work;
            }
            else if (work > 0)
            {
                ans = ans && GameLogic.cumWork >= work;
            }
            if (play < 0)
            {
                ans = ans && GameLogic.cumHobbies <= -play;
            }
            else if (play > 0)
            {
                ans = ans && GameLogic.cumHobbies >= play;
            }
            if (social < 0)
            {
                ans = ans && GameLogic.cumSocial <= -social;
            }
            else if (social > 0)
            {
                ans = ans && GameLogic.cumSocial >= social;
            }
            if (exercise < 0)
            {
                ans = ans && GameLogic.cumExercise <= -exercise;
            }
            else if (exercise > 0)
            {
                ans = ans && GameLogic.cumExercise >= exercise;
            }

            if (happiness < 0)
            {
                ans = ans && gameLogic.happiness <= -happiness;
            }
            else if (happiness > 0)
            {
                ans = ans && gameLogic.happiness >= happiness;
            }
            if (health < 0)
            {
                ans = ans && gameLogic.health <= -health;
            }
            else if (health > 0)
            {
                ans = ans && gameLogic.health >= health;
            }
            if (money < 0)
            {
                ans = ans && gameLogic.money <= -money;
            }
            else if (money > 0)
            {
                ans = ans && gameLogic.money >= money;
            }
            if (meaning < 0)
            {
                ans = ans && gameLogic.meaning <= -meaning;
            }
            else if (meaning > 0)
            {
                ans = ans && gameLogic.meaning >= meaning;
            }
            return ans;
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
        Send("You are born. Hello, world.");
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
        newEvent = new DialogueEvent(1, 100, 1);
        newEvent.SetExercise(0.36f);
        newEvent.SetMessage("You learn to crawl.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(1, 100, 1);
        newEvent.SetMessage("You learn to talk. Your first word is " + firstWords[UnityEngine.Random.Range(0, firstWords.Length)] + ".");
        newEvent.SetSocial(0.72f);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(6, 11, 1);
        newEvent.SetMessage("You are an extremely healthy child.");
        newEvent.SetHealth(4f);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(2, 10, 1);
        newEvent.SetMessage("You feel sad for the first time. You don't know why.");
        newEvent.SetHappiness(-8f);
        newEvent.SetMeaningChange(2);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(2, 11, 1);
        newEvent.SetMessage("You have a newfound appreciation for " + hobbies[UnityEngine.Random.Range(0, hobbies.Length)] + ".");
        newEvent.SetPlay(1.5f);
        newEvent.SetMeaningChange(10);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(7, 7, 1);
        newEvent.SetMessage("You have an 7th birthday party. It's alright.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(6, 13), 14, 0.25f);
        newEvent.SetMessage("You find £5 on the floor. You take it.");
        newEvent.SetMoneyChange(5f);
        newEvent.SetMeaningChange(-2);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(8, 11, 0.7f);
        newEvent.SetMessage("Your best friend moves away. You promise you'll stay in touch.");
        newEvent.SetMeaningChange(1);
        dialogueEvents.Add(newEvent);

        //adolescent
        newEvent = new DialogueEvent(11, 12, 1);
        newEvent.SetMessage("You start at a new school. You hate waking up so early.");
        newEvent.SetHappinessChange(-5);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(11.5f, 12, 0.6f);
        newEvent.SetMessage("You got a A on your first test. It feels good.");
        newEvent.SetWork(2f);
        newEvent.SetHappinessChange(5);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(12, 13), 14, 0.6f);
        newEvent.SetMessage("You start at a new school. You hate waking up so early.");
        newEvent.SetHappinessChange(-5);
        dialogueEvents.Add(newEvent);

        //adult
        newEvent = new DialogueEvent(21, 25, 0.8f);
        newEvent.SetMessage("You move back home. You start applying for 'real' jobs.");
        newEvent.SetMeaningChange(-3);
        newEvent.SetHappinessChange(-5);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(21, 30, 0.25f);
        newEvent.SetMessage("You catch up with old classmates. Everyone got a job in banking.");
        newEvent.SetMeaningChange(4);
        newEvent.SetHappinessChange(-5);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(21, 28, 0.6f);
        newEvent.SetMessage("You take a job you're not excited about. You need the money.");
        newEvent.SetMeaningChange(-7);
        newEvent.SetHappinessChange(-5);
        newEvent.SetMoneyChange(15f);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(23, 40, 1f);
        newEvent.SetMessage("You start a new personal project. It's " + personalProjects[UnityEngine.Random.Range(0, personalProjects.Length)] + ".");
        newEvent.SetPlay(3.78f);
        newEvent.SetMeaningChange(10f);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(24, 28, 1f);
        newEvent.SetMessage("You move into a flat. It smells like wet rags.");
        newEvent.SetMoney(40);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(23, 30, 0.25f);
        newEvent.SetMessage("You get your first payslip. It's less than you expected.");
        newEvent.SetMoneyChange(15);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(25, 29, 0.6f);
        newEvent.SetMessage("You’re asked: “Where do you see yourself in five years?” You lie.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(28, 36, 0.6f);
        newEvent.SetMessage("You stay at your boring job longer than planned. The days blur into a loop.");
        newEvent.SetMeaningChange(-7);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(25, 32, 0.5f);
        newEvent.SetMessage("You wonder if you should go back to school.");
        newEvent.SetMeaning(25);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(27, 35, 0.2f);
        newEvent.SetMessage("Your old friends from school start getting married. It feels weird.");
        newEvent.SetMoneyChange(-3f);
        dialogueEvents.Add(newEvent);
        Debug.Log(dialogueEvents.Count);

        newEvent = new DialogueEvent(27, 37, 0.2f);
        newEvent.SetMessage("You meet someone invigorating at a birthday party. You talk for hours.");
        newEvent.SetSocial(4.5f);
        newEvent.SetHappinessChange(10f);
        newEvent.SetMeaningChange(4f);
        dialogueEvents.Add(newEvent);
        
        newEvent = new DialogueEvent(29, 35, 0.5f);
        newEvent.SetMessage("You get a pet. You talk to them more than people.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(26, 32, 0.25f);
        newEvent.SetMessage("You buy your first real piece of furniture. It feels adult.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(22, 28, 0.25f);
        newEvent.SetMessage("You say I love you to that someone and wait to hear it back.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(25, 32, 0.6f);
        newEvent.SetMessage("You miss a doctor’s appointment and don’t reschedule it.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(22, 22, 1f);
        newEvent.SetMessage("You lose someone suddenly. You weren’t ready");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(29, 34, 0.25f);
        newEvent.SetMessage("You’re offered a better job in a city you don’t love.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(30, 35, 0.25f);
        newEvent.SetMessage("You move again. You’re better at packing this time.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(30, 36, 0.2f);
        newEvent.SetMessage("Your partner says you sound like your parents.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(26, 38, 0.7f);
        newEvent.SetMessage("You forget your friend's birthday.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(31, 39, 0.6f);
        newEvent.SetMessage("You realize you’re not young anymore, but you’re not old either.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(30, 35, 0.3f);
        newEvent.SetMessage("You finally fix that thing in the house that’s been broken for years.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(30, 42, 0.3f);
        newEvent.SetMessage("You go on holiday. You spend most of it trying to relax.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(31, 43, 0.3f);
        newEvent.SetMessage("You host a dinner party. The plates don’t match, but no one cares.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(36, 44, 0.7f);
        newEvent.SetMessage("You watch your parents age. The weight of time passing starts to feel real.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(39, 48, 0.25f);
        newEvent.SetMessage("You start getting invited to more kids’ birthdays than weddings.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(40, 48, 0.2f);
        newEvent.SetMessage("You miss a work deadline. Nothing explodes.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(42, 47, 0.5f);
        newEvent.SetMessage("You buy a really good vacuum. You tell people about it");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(43, 49, 0.1f);
        newEvent.SetMessage("You think about your ex during a layover.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(40, 50, 0.3f);
        newEvent.SetMessage("You realize your childhood friends have become strangers.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(45, 51, 0.25f);
        newEvent.SetMessage("You go out for one drink. You have a hangover the next day.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(27, 55, 0.1f);
        newEvent.SetMessage("You cancel plans because you're tired. You feel a little guilty.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(35, 60, 0.1f);
        newEvent.SetMessage("You find your first grey hair. You leave it.");
        dialogueEvents.Add(newEvent);
        
        newEvent = new DialogueEvent(38, 47, 0.1f );
        newEvent.SetMessage("You drive your kid to school in silence. They don’t say goodbye.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(37, 52, 0.05f);
        newEvent.SetMessage("You wonder if you’re a good parent.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(40, 58, 0.1f);
        newEvent.SetMessage("You wake up before everyone else. You enjoy the quiet.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(38, 59, 0.1f);
        newEvent.SetMessage("You help with homework you don’t understand.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(35, 59, 0.3f);
        newEvent.SetMessage("You throw a birthday party. It’s more for them than you.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(50, 59, 0.2f);
        newEvent.SetMessage("You stay up late folding laundry.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(45, 59, 0.1f);
        newEvent.SetMessage("You realize your kids don’t need you as much anymore");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(47, 59, 0.1f);
        newEvent.SetMessage("You and your partner sit across from each other in silence");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(52, 52, 1f);
        newEvent.SetMessage("You help your last child pack. Their room feels too quiet.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(41, 59, 0.05f);
        newEvent.SetMessage("You split up. It’s not dramatic. Just inevitable");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(42, 59, 0.3f);
        newEvent.SetMessage("You take a spontaneous trip. No one asks where you’re going.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(41, 59, 0.1f);
        newEvent.SetMessage("You finally say no without explaining why or feeling guilt");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(42, 59, 0.2f);
        newEvent.SetMessage("You start doing the thing you used to say you never had time for.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(58, 59, 0.2f);
        newEvent.SetMessage("You wake up and feel excited. It surprises you.");
        dialogueEvents.Add(newEvent);

        //senior dialogue

        newEvent = new DialogueEvent(60, 60, 1f);
        newEvent.SetMessage("You buy new dishes. Not because you need them, but because you want them.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(60, 60, 1f);
        newEvent.SetMessage("You reconnect with old friends. You all look older.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(60, 60, 1f);
        newEvent.SetMessage("You lose someone you thought would live forever.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(60, 60, 1f);
        newEvent.SetMessage("You take better care of your body — not to change it, but to thank it.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(60, 60, 1f);
        newEvent.SetMessage("You start growing things. Tomatoes, microgreens, patience.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(60, 60, 1f);
        newEvent.SetMessage("You go to a high school reunion. No one is who you remember — not even you.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(60, 60, 1f);
        newEvent.SetMessage("You get glasses. You actually like how they look.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(60, 60, 1f);
        newEvent.SetMessage("You read books you never finished the first time.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(60, 60, 1f);
        newEvent.SetMessage("You start mentoring someone younger. They listen.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(60, 60, 1f);
        newEvent.SetMessage("You sit on a train with nowhere urgent to be.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(60, 60, 1f);
        newEvent.SetMessage("You tell stories that make you smile and others cry.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(60, 60, 1f);
        newEvent.SetMessage("You hold hands with someone you’ve loved for decades.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(60, 60, 1f);
        newEvent.SetMessage("You find peace in routine, even when it’s slow.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(60, 60, 1f);
        newEvent.SetMessage("You surprise yourself by learning something new.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(60, 60, 1f);
        newEvent.SetMessage("You realize love doesn’t end — it changes form.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(60, 60, 1f);
        newEvent.SetMessage("You find comfort in silence, and sometimes in company.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(60, 60, 1f);
        newEvent.SetMessage("You say goodbye more times than you expected.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(60, 60, 1f);
        newEvent.SetMessage("You close your eyes and feel the world one last time.");
        dialogueEvents.Add(newEvent);

        Debug.Log(dialogueEvents.Count);
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
        hapGain += Mathf.Min((float)energy, 0f) * scaling * energyPenaltyMultipliers[0];
        hltGain += Mathf.Min((float)energy, 0f) * scaling * energyPenaltyMultipliers[1];
        monGain += Mathf.Min((float)energy, 0f) * scaling * energyPenaltyMultipliers[2];
        mngGain += Mathf.Min((float)energy, 0f) * scaling * energyPenaltyMultipliers[3];
        if (happiness < 0 && hapGain < 0) hapGain = 0;
        if (health < 0 && hltGain < 0) hltGain = 0;
        if (money < 0 && monGain < 0) monGain = 0;
        if (meaning < 0 && mngGain < 0) mngGain = 0;
        happinessDisplay.Rate(hapGain);
        healthDisplay.Rate(hltGain);
        moneyDisplay.Rate(monGain);
        meaningDisplay.Rate(mngGain);

        happiness += hapGain;
        health += hltGain;
        money += monGain;
        meaning += mngGain;
        
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
