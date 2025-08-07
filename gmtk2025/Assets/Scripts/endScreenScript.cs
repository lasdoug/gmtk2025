using UnityEngine;
using TMPro;

public class endScreenScript : MonoBehaviour
{
    public TextMeshProUGUI myText;
    private float cumWork = 0;
    private float cumHobbies = 0;
    private float cumSocial = 0;
    private float cumExercise = 0;
    private float cumHappiness;
    private float cumHealth;
    private float cumMoney;
    private float cumMeaning;
    public int age;
    private int[] ageCompare = { 20, 40, 60, 100 };
    private float[] cumHappinessCompare = { -1f, 0f, 0.6f, 1.2f, 2.0f, 3.0f, 4.0f };
    private float[] cumHealthCompare = { 0.6f, 1.2f, 2.0f , 3.2f, 4.1f };
    private float[] cumMoneyCompare = { 0.4f, 1.0f, 1.7f, 2.6f, 3.6f, 4.6f };
    private float[] cumMeaningCompare = { 0.15f, 0.4f, 0.7f, 1.0f };

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
        cumHappiness = (GameLogic.cumHappiness)/(float)age;
        cumHealth = (GameLogic.cumHealth)/(float)age;
        cumMoney = (GameLogic.cumMoney)/(float)age;
        cumMeaning = (GameLogic.cumMeaning)/(float)age;

        //for testing
        //age = Random.Range(0, 130);
        //cumHappiness = Random.Range(-2.0f, 5.0f);
        //cumHealth = Random.Range(0f, 5.0f);
        //cumMoney = Random.Range(0f, 5f);
        //cumMeaning = Random.Range(0f, 1.2f);
        
        print("year: " + GameLogic.year);
        print("Age: " + age);
        print("Happiness: " + cumHappiness);
        print("Health: " + cumHealth);
        print("Money: " + cumMoney);
        print("Meaning: " + cumMeaning);

        //Make Text String
        myText.text = MakeText();
    }

    string MakeText()
    {
        string cAge = "";
        string cHappy = "";
        string cHealth = "";
        string cMoney = "";
        string cMeaning = "";
        float score = Mathf.Floor(((cumHappiness + cumHealth + cumMoney + cumMeaning)*age));
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
            cAge = "until middle age";
        }
        else if (age < ageCompare[3])
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
            cHappy = "the saddest person to ever live";  
        }
        else if (cumHappiness < cumHappinessCompare[1])
        {
            cHappy = "had no friends";
        }
        else if (cumHappiness < cumHappinessCompare[2])
        {
            cHappy = "sad";
        }
        else if (cumHappiness < cumHappinessCompare[3])
        {
            cHappy = "had a small friend group";
        }
        else if (cumHappiness < cumHappinessCompare[4])
        {
            cHappy = "a happy person";
        }
        else if (cumHappiness < cumHappinessCompare[5])
        {
            cHappy = "very happy with loads of friends";
        }
        else if (cumHappiness < cumHappinessCompare[6])
        {
            cHappy = "extremely happy and popular";
        }
        else
        {
            cHappy = "the happiest person to ever live";
        }

        //HEALTH
        if (cumHealth < cumHealthCompare[0])
        {
            cHealth = "unhealthy";
        }
        else if (cumHealth < cumHealthCompare[1])
        {
            cHealth = "a quite unfit";
        }
        else if (cumHealth < cumHealthCompare[2])
        {
            cHealth = "healthy";
        }
        else if (cumHealth < cumHealthCompare[3])
        {
            cHealth = "a regular at your gym";
        }
        else if (cumHealth < cumHealthCompare[4])
        {
            cHealth = "a local sporting icon";
        }
        else
        {
            cHealth = "could run marathons in your sleep";
        }

        //Money
        if (cumMoney < cumMoneyCompare[0])
        {
            cMoney = "had no money to your name";
        }
        else if (cumMoney < cumMoneyCompare[1])
        {
            cMoney = "struggling financially";
        }
        else if (cumMoney < cumMoneyCompare[2])
        {
            cMoney = "doing okay financially";
        }
        else if (cumMoney < cumMoneyCompare[3])
        {
            cMoney = "owned a house and a dog";
        }
        else if (cumMoney < cumMoneyCompare[4])
        {
            cMoney = "owned quite a few nice cars";
        }
        else if (cumMoney < cumMoneyCompare[5])
        {
            cMoney = "had 2 mansions and a private jet";
        }
        else
        {
            cMoney = "frequently went to space as a holiday";
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
        else if (cumMeaning < cumMeaningCompare[3])
        {
            cMeaning = "had many life experiences";
        }
        else
        {
            cMeaning = "achieved enlightenment.";
        }

        return "Score: " + cScore + "     " + " Age: " + age + "\nYou lived " + cAge + andButAgeHappy + "you were " + cHappy
        + ". \nYou were " + cHealth + andButHealthMoney + cMoney + ". \nYou " + cMeaning + ".";
    }

}
