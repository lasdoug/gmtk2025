using UnityEngine;
using TMPro;

public class EndScreenHealth : MonoBehaviour
{
    TextMeshProUGUI Text;
    void Start()
    {
        Text = GetComponent<TextMeshProUGUI>();
        Text.text = Mathf.Floor(GameLogic.health).ToString();
    }
}

