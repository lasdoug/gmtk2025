using TMPro;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public TMP_Text ageText;
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
}
