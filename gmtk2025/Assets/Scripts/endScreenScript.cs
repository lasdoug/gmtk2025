using UnityEngine;
using UnityEngine.WSA;

public class endScreenScript : MonoBehaviour
{

    public float cumWork = 0;
    public float cumHobbies = 0;
    public float cumSocial = 0;
    public float cumExercise = 0;
    public float cumHappiness = 0;
    public float cumHealth = 0;
    public float cumMoney = 0;
    public float cumMeaning = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cumWork = GameLogic.cumWork;
        cumHobbies = GameLogic.cumHobbies;
        cumSocial = GameLogic.cumSocial;
        cumExercise = GameLogic.cumExercise;
        cumHappiness = GameLogic.cumHappiness;
        cumHealth = GameLogic.cumHealth;
        cumMoney = GameLogic.cumMoney;
        cumMeaning = GameLogic.cumMeaning;
    }

    // Update is called once per frame
    void Update()
    {
    
    }
}
