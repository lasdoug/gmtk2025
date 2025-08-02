using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverOverSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    RawImage image;
    Color hoverColour = Color.white;
    Color normalColour = Color.grey;
    public GameObject iconObject;
    public GameObject description;
    bool hasIcon = false;
    public string holding = "";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<RawImage>();
        image.color = normalColour;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = hoverColour;
        if (hasIcon)
        {
            description.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = normalColour;
        if (hasIcon)
        {
            description.SetActive(false);
        }
    }

    public void GiveIcon(Texture2D iconImg, string name, string desc)
    {
        iconObject.SetActive(true);
        iconObject.GetComponent<RawImage>().texture = iconImg;
        description.GetComponent<DescriptionScript>().SetDescription(name, desc);
        hasIcon = true;
        holding = name;
    }

    public void RemoveIcon()
    {
        holding = "";
        hasIcon = false;
        iconObject.SetActive(false);
    }

}
