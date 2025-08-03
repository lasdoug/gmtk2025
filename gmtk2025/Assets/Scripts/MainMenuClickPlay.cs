using UnityEngine;

public class MainMenuClickPlay : MonoBehaviour
{
    public void PlaySound()
    {
        GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>().Play("menuClick");
    }
}
