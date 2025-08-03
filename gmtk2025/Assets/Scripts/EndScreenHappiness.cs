using UnityEngine;
using TMPro;

public class EndScreenHappiness : MonoBehaviour
{
    TextMeshProUGUI hText;
    void Start()
    {
        hText = GetComponent<TextMeshProUGUI>();
        if (GameLogic.happiness < 0)
        {
            hText.text = "0";
        }
        else
        {
            hText.text = Mathf.Floor(GameLogic.happiness).ToString();
        }
    }
}
