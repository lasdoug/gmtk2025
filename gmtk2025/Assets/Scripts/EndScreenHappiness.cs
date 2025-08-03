using UnityEngine;
using TMPro;

public class EndScreenHappiness : MonoBehaviour
{
    TextMeshProUGUI hText;
    void Start()
    {
        hText = GetComponent<TextMeshProUGUI>();
        hText.text = Mathf.Floor(GameLogic.happiness).ToString();
    }
}
