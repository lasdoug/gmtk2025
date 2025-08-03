using UnityEngine;
using TMPro;

public class EndScreenMoney : MonoBehaviour
{
    TextMeshProUGUI Text;
    void Start()
    {
        Text = GetComponent<TextMeshProUGUI>();
        Text.text = Mathf.Floor(GameLogic.money).ToString();
    }
}
