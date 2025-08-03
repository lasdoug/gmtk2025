using UnityEngine;
using TMPro;

public class EndScreenMoney : MonoBehaviour
{
    TextMeshProUGUI Text;
    void Start()
    {
        Text = GetComponent<TextMeshProUGUI>();
        if (GameLogic.money < 0)
        {
            Text.text = "0";
        }
        else
        {
            Text.text = Mathf.Floor(GameLogic.money).ToString();
        }
    }
}
