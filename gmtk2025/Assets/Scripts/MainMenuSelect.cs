using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSelect : MonoBehaviour
{
    public void SwitchScene(int i)
    {
        SceneManager.LoadScene(i);
    }
}
