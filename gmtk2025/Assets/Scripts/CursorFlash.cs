using UnityEngine;
using UnityEngine.UI;

public class CursorFlash : MonoBehaviour
{
    public float onFor = 0.6f;
    public float offFor = 0.4f;
    float cnt;
    bool isOn = true;
    RawImage image;

    void Start()
    {
        image = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        cnt += Time.deltaTime;
        if (isOn)
        {
            if (cnt >= onFor)
            {
                isOn = false;
                cnt -= onFor;
                image.color = new Color(1, 1, 1, 0);
            }
        }
        else
        {
            if (cnt >= offFor)
            {
                isOn = true;
                cnt -= offFor;
                image.color = new Color(1, 1, 1, 1);
            }
        }
    }
}
