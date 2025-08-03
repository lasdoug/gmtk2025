using UnityEngine;
using TMPro;

public class EndScreenMeaning : MonoBehaviour
{
    TextMeshProUGUI Text;
    void Start()
    {
        Text = GetComponent<TextMeshProUGUI>();
        if (GameLogic.meaning < 0)
        {
            Text.text = "0";
        }
        else
        {
            Text.text = Mathf.Floor(GameLogic.meaning).ToString();
        }
    }
}
