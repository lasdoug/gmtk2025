using UnityEngine;
using UnityEngine.EventSystems;

public class HoverOverResources : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    GameObject popUpBorder;

    void Start()
    {
        popUpBorder = gameObject.transform.Find("PopUpBorder").gameObject;
        if (popUpBorder != null)
        {
            print("found");
        }
        else
        {
            print("not found");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        print("entered");
        popUpBorder.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        popUpBorder.SetActive(false);
    }
}
