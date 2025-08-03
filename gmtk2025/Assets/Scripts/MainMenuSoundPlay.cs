using Unity.VisualScripting;
using UnityEngine;

public class MainMenuSoundPlay : MonoBehaviour
{
    public void PlaySound()
    {
        GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>().Play("startGame");
    }
}
