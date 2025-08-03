using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    public GameObject pauseOverlay;
    public StatBlockScript happinessDisplay, healthDisplay, moneyDisplay, meaningDisplay;
    public bool isPaused = false;
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
    float burnoutCounter = 0f;
    public static int year = 0;
    public int energy = 0;
    public int maxEnergy = 40;
    public static float happiness = 50f;
    public static float health = 90f;
    public static float money = 0f;
    public static float meaning = 0f;

    bool energyTweening = false;
    DOTweenTMPAnimator animator;
    Sequence energyTextTween;
    NotifScript dialogueBox;

    public float scaling = 0.1f;
    public float[] dropoffMultipliers = { 1, 1, 1, 1 };
    // HAPPINESS , HEALTH , MONEY , MEANING
    float[] workMultipliers = { 0.5f, -0.3f, 0.05f, 0.1f };
    float past16MoneyMultiplier = 0.2f;
    float past21MoneyMultiplier = 0.4f;
    float[] playMultipliers = { 0.4f, 1, -1, 0.1f };
    float[] socialMultipliers = { 0.4f, -1, -1, 0.1f };
    float[] exerciseMultipliers = { 0.1f, 0.4f, -1, 0.1f };
    // Vector3(mean, variance, mode)
    // mode: 0 no penalty, 1 penalty
    Vector3[] workWindows = { new Vector3(6, 2, 1), new Vector3(20, 15, 0), new Vector3(20, 19, 1), new Vector3(12, 3, 0) };
    Vector3[] playWindows = { new Vector3(20, 20, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(17, 3, 0) };
    Vector3[] socialWindows = { new Vector3(20, 15, 1), new Vector3(20, 2, 0), new Vector3(20, 6, 0), new Vector3(20, 5, 0) };
    Vector3[] exerciseWindows = { new Vector3(11, 6, 1), new Vector3(20, 18, 1), new Vector3(20, 5, 0), new Vector3(0, 0, 0) };
    float[] flatReductions = { -0.1f, -0.1f, -0.8f, 0f };
    float[] energyPenaltyMultipliers = { 1, 1, 1, 0.05f };

    List<Func<float>> calculateHappiness = new();
    List<Func<float>> calculateHealth = new();
    List<Func<float>> calculateMoney = new();
    List<Func<float>> calculateMeaning = new();

    public static float cumWork, cumHobbies, cumSocial, cumExercise, cumHappiness, cumHealth, cumMoney, cumMeaning;

    string[] achievements = { "helloworld", "helloworldagain", "superchargedbaby", "centenarian", "truemeaning", "immovableobject", "silent", "earlyactor", "playful", "desertislanddisc" };
    public List<HoverOverSlot> slots;
    bool hobbyFlag = false;
    List<DialogueEvent> dialogueEvents = new();
    string[] bands = { "SYSTEM OF A DOWN", "BLINK-182", "LINKIN PARK", "GREEN DAY", "WEIRD AL", "MUSE", "THE ARCTIC MONKEYS", "EZRA COLLECTIVE", "PORTISHEAD", "MASSIVE ATTACK", "THE PRODIGY", "ALT-J", "FOO FIGHTERS", "SOUNDGARDEN" };
    string[] firstWords = { "THROAT", "RICE", "CAR", "NOODLES", "MOMMA", "DADDA", "CELERY", "FRESCO", "CONGLOMERATE", "NO" };
    string[] hobbies = { "DINOSAURS", "TRUCKS", "THE STOCK MARKET", "MAGIC", "MAGIC THE GATHERING", "BUGS", "BASS GUITAR", "MAHJONG" };
    string[] personalProjects = { "A FILM ABOUT DUST", "AN INDIE GAME ABOUT FOXES", "AN EP OF HOUSE MUSIC", "A PAINTING OF YOUR HOUSE" };
    string[] lateLifeThings = { "MODULAR SYNTHESIS", "ANALOG CAMERAS", "ARTISNAL COFFEE", "FERMENTATION" };
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
                    GameLogic.happiness += happinesschange;
                    GameLogic.health += healthchange;
                    GameLogic.money += moneychange;
                    GameLogic.meaning += meaningchange;
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
                ans = ans && GameLogic.happiness <= -happiness;
            }
            else if (happiness > 0)
            {
                ans = ans && GameLogic.happiness >= happiness;
            }
            if (health < 0)
            {
                ans = ans && GameLogic.health <= -health;
            }
            else if (health > 0)
            {
                ans = ans && GameLogic.health >= health;
            }
            if (money < 0)
            {
                ans = ans && GameLogic.money <= -money;
            }
            else if (money > 0)
            {
                ans = ans && GameLogic.money >= money;
            }
            if (meaning < 0)
            {
                ans = ans && GameLogic.meaning <= -meaning;
            }
            else if (meaning > 0)
            {
                ans = ans && GameLogic.meaning >= meaning;
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

        newEvent = new DialogueEvent(1, 11, 1);
        newEvent.SetMessage("Your insane work ethic nets you an good acting role. It's gruelling.");
        newEvent.SetWork(3.2f);
        newEvent.SetHappinessChange(-20);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(6, 11, 1);
        newEvent.SetMessage("You are an extremely healthy child.");
        newEvent.SetHealth(175);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(2, 10, 1);
        newEvent.SetMessage("You feel sad for the first time. You don't know why.");
        newEvent.SetHappinessChange(-8f);
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
        newEvent.SetMeaningChange(1);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(11.5f, 12, 0.6f);
        newEvent.SetMessage("You got a C on your first test. You might work harder.");
        newEvent.SetWork(-0.5f);
        newEvent.SetHappinessChange(-5);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(12f, 13), 14, 0.7f);
        newEvent.SetMessage("You and your new mates discover " + bands[UnityEngine.Random.Range(0, bands.Length)] + ".");
        newEvent.SetSocial(1.9f);
        newEvent.SetMeaningChange(2);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(12f, 13), 14, 1);
        newEvent.SetMessage("You struggle a bit to make new friends.");
        newEvent.SetSocial(-0.8f);
        newEvent.SetMeaningChange(-1);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(11.5f, 17), 18, 0.4f);
        newEvent.SetMessage("You stay out past curfew.");
        newEvent.SetSocial(3f);
        newEvent.SetMeaningChange(2);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(11.5f, 17), 18, 0.4f);
        newEvent.SetMessage("You have your first big argument with your parents.");
        newEvent.SetSocial(2f);
        newEvent.SetHappinessChange(-5);
        newEvent.SetMeaningChange(1);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(13.5f, 17), 18, 0.4f);
        newEvent.SetMessage("You fall in love for the first time. It's a scary feeling.");
        newEvent.SetMeaningChange(5);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(13.5f, 17), 18, 0.4f);
        newEvent.SetMessage("You are invited to join the school's swimming team.");
        newEvent.SetExercise(3.5f);
        newEvent.SetHappinessChange(5);
        newEvent.SetMeaningChange(3);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(13.5f, 17), 18, 0.4f);
        newEvent.SetMessage("You stay up all night with a friend, just talking.");
        newEvent.SetMeaningChange(3);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(13.5f, 17), 18, 0.5f);
        newEvent.SetMessage("You are offered a shoulder to cry on. It helps.");
        newEvent.SetHappiness(-30);
        newEvent.SetMeaningChange(6);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(13.5f, 18), 18, 1);
        newEvent.SetMessage("You are top of your class.");
        newEvent.SetWork(4.5f);
        newEvent.SetHappinessChange(5);
        newEvent.SetMeaningChange(1);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(13.5f, 17), 18, 0.4f);
        newEvent.SetMessage("You go ice skating. You fall over, and get up again.");
        newEvent.SetPlay(2.5f);
        newEvent.SetHappinessChange(5);
        newEvent.SetMeaningChange(1);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(13.5f, 16), 18, 1);
        newEvent.SetMessage("You watch your hundredth movie. It resonates.");
        newEvent.SetPlay(4.1f);
        newEvent.SetHappinessChange(-5);
        newEvent.SetMeaningChange(5);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(13.5f, 16), 18, 1);
        newEvent.SetMessage("You develop obsessions.");
        newEvent.SetPlay(6f);
        newEvent.SetMeaningChange(10);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(15.5f, 19), 20, 0.35f);
        newEvent.SetMessage("You cry for no reason and then laugh hysterically 5 minutes later.");
        newEvent.SetHappinessChange(-5);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(15.5f, 19), 20, 0.4f);
        newEvent.SetMessage("You pretend to like something just to fit in.");
        newEvent.SetHappinessChange(5);
        newEvent.SetMeaningChange(-5);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(15.5f, 19), 20, 0.7f);
        newEvent.SetMessage("You get a part time job. The money is nice.");
        newEvent.SetMoney(30);
        newEvent.SetHappinessChange(5);
        newEvent.SetMeaningChange(2);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(15.5f, 30), 30, 0.5f);
        newEvent.SetMessage("You and a stranger laugh hard at a huge toad.");
        newEvent.SetHappiness(70);
        newEvent.SetHappinessChange(5);
        newEvent.SetMeaningChange(4);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(17.5f, 55), 56, 0.5f);
        newEvent.SetMessage("You lie in the sun for longer than you planned.");
        newEvent.SetHappiness(70);
        newEvent.SetHappinessChange(5);
        newEvent.SetMeaningChange(4);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(15.5f, 21), 22, 0.4f);
        newEvent.SetMessage("Your friend skims a rock all the way across a pond.");
        newEvent.SetSocial(3);
        newEvent.SetHappinessChange(5);
        newEvent.SetMeaningChange(2);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(17.5f, 55), 56, 0.2f);
        newEvent.SetMessage("It rains, but you have a surprisingly nice time inside.");
        newEvent.SetHappinessChange(5);
        newEvent.SetMeaningChange(2);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(17.5f, 55), 56, 0.1f);
        newEvent.SetMessage("You see someone who looks just like you.");
        newEvent.SetMeaningChange(2);
        dialogueEvents.Add(newEvent);

        //uni
        newEvent = new DialogueEvent(17.1f, 19, 1f);
        newEvent.SetMessage("School is almost over. You don't know what you want to do next.");
        newEvent.SetPlay(-3f);
        newEvent.SetMeaningChange(-5);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(17.1f, 19, 1f);
        newEvent.SetMessage("School is almost over. You want to pursue your interests.");
        newEvent.SetPlay(3.1f);
        newEvent.SetMeaningChange(5);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(17.5f, 19, 0.7f);
        newEvent.SetMessage("Prom is coming up. You invite your crush.");
        newEvent.SetSocial(3.5f);
        newEvent.SetMeaningChange(1);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(17.5f, 19, 0.7f);
        newEvent.SetMessage("Prom is coming up. You buy an outfit.");
        newEvent.SetSocial(-3f);
        newEvent.SetMoneyChange(-5);
        newEvent.SetMeaningChange(1);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(17.5f, 22), 23, 0.4f);
        newEvent.SetMessage("You get your heart broken. You pretend it's fine.");
        newEvent.SetSocial(2f);
        newEvent.SetHappinessChange(-15);
        newEvent.SetMeaningChange(2);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(18, 20, 0.4f);
        newEvent.SetMessage("You move into student housing. The walls are thin.");
        newEvent.SetWork(0.2f);
        newEvent.SetMeaningChange(1);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(18.5f, 20), 21, 0.3f);
        newEvent.SetMessage("Do you like your degree?");
        newEvent.SetWork(0.2f);
        newEvent.SetHappiness(-50);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(18.5f, 20), 21, 1);
        newEvent.SetMessage("You get your first job out of school. It's ok.");
        newEvent.SetWork(-0.2f);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(18.5f, 20), 21, 0.35f);
        newEvent.SetMessage("You skip breakfast and live off coffee for three days.");
        newEvent.SetHealthChange(-10);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(18.5f, 20), 21, 0.35f);
        newEvent.SetMessage("You hook up with someone at a party. You don't talk after.");
        newEvent.SetMeaningChange(1);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(18.5f, 20), 21, 0.35f);
        newEvent.SetMessage("You sit in a lecture that changes how you see the world.");
        newEvent.SetWork(0.2f);
        newEvent.SetMeaningChange(3);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(18.5f, 20), 21, 0.35f);
        newEvent.SetMessage("You take a walk at night.");
        newEvent.SetMeaningChange(1);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(20.1f, 21), 22, 0.5f);
        newEvent.SetMessage("It's your final year of study. You feel anxious sometimes.");
        newEvent.SetWork(0.3f);
        newEvent.SetHappinessChange(-5);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(UnityEngine.Random.Range(20.1f, 21), 22, 0.5f);
        newEvent.SetMessage("You submit your final assignment. Sleeping feels nice.");
        newEvent.SetWork(0.3f);
        newEvent.SetHealthChange(-5);
        newEvent.SetHappinessChange(5);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(21, 22, 0.5f);
        newEvent.SetMessage("There are a lot of people at your graduation.");
        newEvent.SetWork(0.3f);
        newEvent.SetHappinessChange(5);
        dialogueEvents.Add(newEvent);

        //adult
        newEvent = new DialogueEvent(20, 25, 0.2f);
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

        newEvent = new DialogueEvent(23, 40, 0.5f);
        newEvent.SetMessage("You start a new personal project. It's " + personalProjects[UnityEngine.Random.Range(0, personalProjects.Length)] + ".");
        newEvent.SetPlay(3.78f);
        newEvent.SetMeaningChange(10f);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(24, 28, 1f);
        newEvent.SetMessage("You move into a flat. It smells like wet rags.");
        newEvent.SetMoney(40);
        newEvent.SetHappinessChange(-10);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(23, 30, 0.25f);
        newEvent.SetMessage("You get your first payslip. It's less than you expected.");
        newEvent.SetMoneyChange(15);
        newEvent.SetHappinessChange(-10);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(25, 29, 0.3f);
        newEvent.SetMessage("You’re asked: “Where do you see yourself in five years?” You lie.");
        newEvent.SetWork(10);
        newEvent.SetMeaningChange(-1);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(28, 36, 0.15f);
        newEvent.SetMessage("You stay at your job longer than planned. The days blur into a LOOP.");
        newEvent.SetWork(11.2f);
        newEvent.SetHappinessChange(-10);
        newEvent.SetMeaningChange(-7);
        newEvent.SetMoneyChange(20);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(25, 32, 0.2f);
        newEvent.SetMessage("You wonder if you should go back to school.");
        newEvent.SetMeaning(25);
        newEvent.SetPlay(4.5f);
        newEvent.SetMeaningChange(3);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(27, 35, 0.1f);
        newEvent.SetMessage("Your old friends from school start getting married. It feels weird.");
        newEvent.SetMoneyChange(-3f);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(27, 37, 0.2f);
        newEvent.SetMessage("You meet someone invigorating at a birthday party. You talk for hours.");
        newEvent.SetSocial(4.5f);
        newEvent.SetHappinessChange(10f);
        newEvent.SetMeaningChange(4f);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(29, 35, 0.1f);
        newEvent.SetMessage("You get a pet. You talk to them more than people.");
        newEvent.SetHappinessChange(20f);
        newEvent.SetMeaningChange(10f);
        newEvent.SetMoneyChange(-30);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(26, 32, 0.2f);
        newEvent.SetMessage("You buy your first real piece of furniture. It feels adult.");
        newEvent.SetMoney(75);
        newEvent.SetMoneyChange(-5);
        newEvent.SetMeaning(2);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(22, 30, 0.1f);
        newEvent.SetMessage("You say I love you to that someone and wait to hear it back.");
        newEvent.SetPlay(7);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(25, 32, 0.6f);
        newEvent.SetMessage("You miss a doctor’s appointment and don't reschedule it.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(22, 22, 1f);
        newEvent.SetMessage("You lose someone suddenly. You weren’t ready");
        newEvent.SetMeaning(7);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(29, 34, 0.2f);
        newEvent.SetMessage("You’re offered a better job in a city you don’t love.");
        newEvent.SetWork(17.4f);
        newEvent.SetMoneyChange(20);
        newEvent.SetMeaningChange(-4);
        newEvent.SetHappinessChange(-4);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(30, 35, 0.05f);
        newEvent.SetMessage("You move again. You’re better at packing this time.");
        newEvent.SetHappinessChange(5);
        newEvent.SetMoneyChange(-5);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(30, 36, 0.05f);
        newEvent.SetMessage("Your partner says you sound like your parents.");
        newEvent.SetHappinessChange(-5);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(26, 38, 0.1f);
        newEvent.SetMessage("You forget your friend's birthday.");
        newEvent.SetWork(23f);
        newEvent.SetMeaningChange(-3);
        newEvent.SetHappinessChange(-3);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(31, 39, 0.1f);
        newEvent.SetMessage("You realize you’re not young anymore, but you’re not old either.");
        newEvent.SetMeaningChange(5);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(30, 35, 0.05f);
        newEvent.SetMessage("You finally fix something that has been broken for years.");
        newEvent.SetMeaningChange(1);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(30, 42, 0.15f);
        newEvent.SetMessage("You go on holiday. You spend most of it trying to relax.");
        newEvent.SetMoney(200);
        newEvent.SetPlay(18);
        newEvent.SetMoneyChange(-50);
        newEvent.SetHappinessChange(5);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(31, 43, 0.1f);
        newEvent.SetMessage("You host a dinner party. The plates don’t match, but no one cares.");
        newEvent.SetSocial(15.2f);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(36, 44, 0.1f);
        newEvent.SetMessage("You watch your parents age. The weight of time passing starts to feel real.");
        newEvent.SetMeaning(30);
        newEvent.SetMeaningChange(3);
        newEvent.SetHealthChange(10);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(39, 48, 0.1f);
        newEvent.SetMessage("You start getting invited to more kids’ birthdays than weddings.");
        newEvent.SetSocial(20);
        newEvent.SetHappinessChange(10);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(21, 55, 0.05f);
        newEvent.SetMessage("You miss a work deadline. Nothing explodes.");
        newEvent.SetPlay(16.8f);
        newEvent.SetMeaningChange(3);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(42, 43, 0.5f);
        newEvent.SetMessage("You buy a really good vacuum. You tell people about it");
        newEvent.SetMeaningChange(1);
        newEvent.SetMoney(60);
        newEvent.SetSocial(9);
        newEvent.SetHappinessChange(3);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(40, 50, 0.1f);
        newEvent.SetMessage("You realize your childhood friends have become strangers.");
        newEvent.SetSocial(12.5f);
        newEvent.SetHappinessChange(-5);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(45, 51, 0.2f);
        newEvent.SetMessage("You go out for one drink. You have a hangover the next day.");
        newEvent.SetSocial(16);
        newEvent.SetHealthChange(-2);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(36, 55, 0.1f);
        newEvent.SetMessage("You cancel plans because you're tired. You feel a little guilty.");
        newEvent.SetHappiness(25);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(38, 60, 0.1f);
        newEvent.SetMessage("You find your first grey hair. You leave it.");
        newEvent.SetMeaningChange(2);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(40, 58, 0.1f);
        newEvent.SetMessage("You wake up before everyone else. You enjoy the quiet.");
        newEvent.SetMeaningChange(1);
        newEvent.SetWork(16.4f);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(35, 59, 0.3f);
        newEvent.SetMessage("You throw a birthday party. It is more for them than you.");
        newEvent.SetHappinessChange(10f);
        newEvent.SetMeaningChange(2f);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(50, 59, 0.1f);
        newEvent.SetMessage("You stay up late folding laundry.");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(47, 59, 0.1f);
        newEvent.SetMessage("You and your partner sit across from each other in silence");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(38, 59, 0.05f);
        newEvent.SetMessage("You split up. It’s not dramatic.");
        newEvent.SetWork(13.2f);
        newEvent.SetMeaningChange(4);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(42, 59, 0.1f);
        newEvent.SetMessage("You take a spontaneous trip. No one asks where you are.");
        newEvent.SetPlay(12f);
        newEvent.SetHappinessChange(20f);
        newEvent.SetMeaning(4);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(41, 59, 0.05f);
        newEvent.SetMessage("You finally say no without explaining why or feeling guilt.");
        newEvent.SetMeaning(30);
        newEvent.SetHappinessChange(10);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(42, 59, 0.05f);
        newEvent.SetMessage("You start doing the thing you never had time for: " + lateLifeThings[UnityEngine.Random.Range(0, lateLifeThings.Length)] + ".");
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(58, 62, 0.3f);
        newEvent.SetMessage("You wake up and feel excited. It surprises you.");
        newEvent.SetPlay(12);
        newEvent.SetHappiness(60);
        newEvent.SetHappinessChange(5);
        newEvent.SetMeaningChange(5);
        dialogueEvents.Add(newEvent);

        //senior dialogue

        newEvent = new DialogueEvent(60, 60, 0.5f);
        newEvent.SetMessage("You buy new dishes. Not because you need them, but because you want them.");
        newEvent.SetMoney(80);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(61, 61, 1f);
        newEvent.SetMessage("You reconnect with old friends. You all look older.");
        newEvent.SetSocial(18.3f);
        newEvent.SetMeaningChange(3);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(62, 63, 0.4f);
        newEvent.SetMessage("You lose someone you thought would live forever.");
        newEvent.SetHappinessChange(-25);
        newEvent.SetMeaningChange(2);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(60, 75, 0.05f);
        newEvent.SetMessage("You get glasses. You actually like how they look.");
        newEvent.SetHealthChange(5);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(63, 67, 1f);
        newEvent.SetMessage("You read a book that you never finished.");
        newEvent.SetMeaningChange(1);
        newEvent.SetPlay(15);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(50, 68, 1f);
        newEvent.SetMessage("You start mentoring someone younger. They listen.");
        newEvent.SetWork(20);
        newEvent.SetMeaningChange(3);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(60, 70, 1f);
        newEvent.SetMessage("You sit on a train with nowhere urgent to be.");
        newEvent.SetPlay(28);
        newEvent.SetHappinessChange(5);
        newEvent.SetMeaningChange(3);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(65, 75, 1f);
        newEvent.SetMessage("You tell stories that make you smile and others cry.");
        newEvent.SetSocial(28);
        newEvent.SetMeaningChange(5);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(71, 71, 1f);
        newEvent.SetMessage("You hold hands with someone you’ve loved for decades.");
        newEvent.SetSocial(18);
        newEvent.SetPlay(18);
        newEvent.SetMeaningChange(3);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(72, 73, 1f);
        newEvent.SetMessage("You find peace in routine, even when it’s slow.");
        newEvent.SetMeaning(40);
        newEvent.SetMeaningChange(3);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(74, 75, 1f);
        newEvent.SetMessage("You surprise yourself by learning something new.");
        newEvent.SetPlay(26.64f);
        newEvent.SetMeaning(5);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(70, 80, 0.1f);
        newEvent.SetMessage("You realize love doesn’t end — it changes form.");
        newEvent.SetMeaningChange(2);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(70, 80, 0.1f);
        newEvent.SetMessage("You find comfort in silence, and sometimes in company.");
        newEvent.SetSocial(13.5f);
        newEvent.SetMeaningChange(2);
        dialogueEvents.Add(newEvent);

        //extreme resource events
        newEvent = new DialogueEvent(35, 120, 1f);
        newEvent.SetMessage("You start growing things. Tomatoes, microgreens, patience.");
        newEvent.SetMeaning(55);
        dialogueEvents.Add(newEvent);

        newEvent = new DialogueEvent(80, 120, 1f);
        newEvent.SetHealth(3f);
        newEvent.SetHealthChange(-3);
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
            if (year == 16)
            {
                workMultipliers[2] = past16MoneyMultiplier;
            }
            if (year == 21)
            {
                workMultipliers[2] = past21MoneyMultiplier;
            }
            Debug.Log("Cumulative values: " + cumWork + "," + cumHobbies + "," + cumSocial + "," + cumExercise + "," + cumHappiness + "," + cumHealth + "," + cumMoney + "," + cumMeaning);
        }

        if (energyTweening && burnoutCounter < 2f)
        {
            burnoutCounter += Time.deltaTime;
            if (burnoutCounter >= 2f)
            {
                Send("<b>ERROR:<b> You feel you are burning out.");
            }
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
            burnoutCounter = 0f;
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

    // void CheckAchievements()
    // {
    //     if (!slots[6].GetAchieved() && cumSocial < 0.71f && year >= 40)
    //     {
    //         slots[6].SetAchieved();
    //         PlayerPrefs.SetInt(achievements[6], 1);
    //     }
    //     if (!slots[5].GetAchieved() && cumExercise < 0.35f && year >= 20)
    //     {
    //         slots[5].SetAchieved();
    //         PlayerPrefs.SetInt(achievements[5], 1);
    //     }
    //     if (!slots[7].GetAchieved() && cumWork >= 3.2f && year <= 11)
    //     {
    //         slots[7].SetAchieved();
    //         PlayerPrefs.SetInt(achievements[7], 1);
    //     }
    // }

}
