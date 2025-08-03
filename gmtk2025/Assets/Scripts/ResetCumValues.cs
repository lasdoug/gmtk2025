using UnityEngine;

public class ResetCumValues : MonoBehaviour
{

    public void resetCumValues()
    {
        GameLogic.cumWork = 0;
        GameLogic.cumHobbies = 0;
        GameLogic.cumSocial = 0;
        GameLogic.cumExercise = 0;
        GameLogic.happiness = 50;
        GameLogic.health = 90;
        GameLogic.money = 0;
        GameLogic.meaning = 0;
    }
}
