using UnityEngine;
using TMPro;

public class EndScreenMeaning : MonoBehaviour
{
    TextMeshProUGUI Text;
    void Start()
    {
        Text = GetComponent<TextMeshProUGUI>();
        Text.text = Mathf.Floor(GameLogic.meaning).ToString();
    }
}
