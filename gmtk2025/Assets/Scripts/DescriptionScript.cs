using TMPro;
using UnityEngine;

public class DescriptionScript : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text descText;

    public void SetDescription(string name, string desc)
    {
        nameText.text = name;
        descText.text = desc;
    }
}
