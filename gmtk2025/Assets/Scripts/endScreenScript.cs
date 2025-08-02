using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.WSA;
using TMPro;

public class endScreenScript : MonoBehaviour
{
    public TextMeshProUGUI myText;
    private float cumWork = 0;
    private float cumHobbies = 0;
    private float cumSocial = 0;
    private float cumExercise = 0;
    public float cumHappiness = 0f;
    public float cumHealth = 0f;
    public float cumMoney = 0f;
    public float cumMeaning = 0f;
    public int age = 0;
    public int[] ageCompare = { 10, 40, 70 };
    public float[] cumHappinessCompare = { 100, 200, 300 };
    public float[] cumHealthCompare = { 100, 200, 300 };
    public float[] cumMoneyCompare = { 100, 200, 300 };
    public float[] cumMeaningCompare = { 100, 200, 300 };
    private enum personal
    {
        happiness = 0,
        health = 1,
        money = 2,
        meaning = 3
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Get variables from GameLogic.cs
        age = GameLogic.year;
        cumWork = GameLogic.cumWork;
        cumHobbies = GameLogic.cumHobbies;
        cumSocial = GameLogic.cumSocial;
        cumExercise = GameLogic.cumExercise;
        cumHappiness = GameLogic.cumHappiness;
        cumHealth = GameLogic.cumHealth;
        cumMoney = GameLogic.cumMoney;
        cumMeaning = GameLogic.cumMeaning;

        //for testing
        age = Random.Range(0, 130);
        cumHappiness = Random.Range(0, 400);
        cumHealth = Random.Range(0, 400);
        cumMoney = Random.Range(0, 400);
        cumMeaning = Random.Range(0, 400);
    
        //Make Text String
        myText.text = MakeText();
    }

    // Update is called once per frame
    void Update()
    {

    }

    string MakeText()
    {
        string cAge = "";
        string cHappy = "";
        string cHealth = "";
        string cMoney = "";
        string cMeaning = "";
        float score = ((cumHappiness + cumHealth + cumMoney + cumMeaning));
        string cScore = score.ToString();
        string andButAgeHappy = "";
        string andButHealthMoney = "";

        if ((age >= ageCompare[1] && cumHappiness >= cumHappinessCompare[1])
        || (age < ageCompare[1] && cumHappiness < cumHappinessCompare[1]))
        {
            andButAgeHappy = " and ";
        }
        else
        {
            andButAgeHappy = " but ";
        }

        if ((cumHealth >= cumHealthCompare[1] && cumMoney >= cumMoneyCompare[1])
        || (cumHealth < cumHealthCompare[1] && cumMoney < cumMoneyCompare[1]))
        {
            andButHealthMoney = " and ";
        }
        else
        {
            andButHealthMoney = " but ";
        }

        //AGE
        if (age < ageCompare[0])
        {
            cAge = "a short life";
        }
        else if (age < ageCompare[1])
        {
            cAge = "until early age";
        }
        else if (age < ageCompare[2])
        {
            cAge = "a long life";
        }
        else
        {
            cAge = "a ridiculously long life";
        }

        //HAPPINESS
        if (cumHappiness < cumHappinessCompare[0])
        {
            cHappy = "sad";  
        }
        else if (cumHappiness < cumHappinessCompare[1])
        {
            cHappy = "kinda sad";
        }
        else if (cumHappiness < cumHappinessCompare[2])
        {
            cHappy = "happy";
        }
        else
        {
            cHappy = "very happy";
        }

        //HEALTH
        if (cumHealth < cumHealthCompare[0])
        {
            cHealth = "unhealthy";
        }
        else if (cumHealth < cumHealthCompare[1])
        {
            cHealth = "a lil unfit";
        }
        else if (cumHealth < cumHealthCompare[2])
        {
            cHealth = "healthy";
        }
        else
        {
            cHealth = "very healthy";
        }

        //Money
        if (cumMoney < cumMoneyCompare[0])
        {
            cMoney = "poor";
        }
        else if (cumMoney < cumMoneyCompare[1])
        {
            cMoney = "struggling financially";
        }
        else if (cumMoney < cumMoneyCompare[2])
        {
            cMoney = "doing well financially";
        }
        else
        {
            cMoney = "exorbitantly rich";
        }

        //Meaning
        if (cumMeaning < cumMeaningCompare[0])
        {
            cMeaning = "had no life purpose";
        }
        else if (cumMeaning < cumMeaningCompare[1])
        {
            cMeaning = "felt a bit lost";
        }
        else if (cumMeaning < cumMeaningCompare[2])
        {
            cMeaning = "had a strong life direction";
        }
        else
        {
            cMeaning = "were zen like monk";
        }

        return "Score: " + cScore + "\nYou lived " + cAge + andButAgeHappy + "you were " + cHappy
        + ". \nYou were " + cHealth + andButHealthMoney + cMoney + ". \nYou " + cMeaning + ".";
    }

}
