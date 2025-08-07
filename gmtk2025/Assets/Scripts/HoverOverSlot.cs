using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverOverSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    RawImage image;
    Color hoverColour = Color.grey;
    Color normalColour = Color.black;
    public GameObject iconObject;
    public GameObject descriptionBox;
    bool hasIcon = false;
    [SerializeField] string achievementDescription;
    TextMeshProUGUI achievementTextObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<RawImage>();
        image.color = normalColour;

        achievementTextObj = gameObject.transform.Find("Description/TextArea/Desc").GetComponent<TextMeshProUGUI>();
        achievementTextObj.text = achievementDescription;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionBox.SetActive(true);
        achievementTextObj.text = achievementDescription;
        image.color = hoverColour;

        if (!hasIcon)
        {
            achievementTextObj.text = "???";
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = normalColour;
        descriptionBox.SetActive(false);
    }

    public void SetAchieved()
    {
        hasIcon = true;
        iconObject.SetActive(true);
    }

    public bool GetAchieved()
    {
        return hasIcon;
    }

}
