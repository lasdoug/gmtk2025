using UnityEngine;
using TMPro;

public class EndScreenHealth : MonoBehaviour
{
    TextMeshProUGUI Text;
    void Start()
    {
        Text = GetComponent<TextMeshProUGUI>();
        if (GameLogic.health < 0)
        {
            Text.text = "0";
        }
        else
        {
            Text.text = Mathf.Floor(GameLogic.health).ToString();
        }
    }
}

