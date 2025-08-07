using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ResetAchievements : MonoBehaviour
{
    GameLogic logic;

    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<GameLogic>();
    }
    public void ResetAchievementsGained()
    {
        for (int i = 0; i < logic.achievements.Length; i++)
        {
            PlayerPrefs.SetInt(logic.achievements[i], 0);
        }
    }
}
