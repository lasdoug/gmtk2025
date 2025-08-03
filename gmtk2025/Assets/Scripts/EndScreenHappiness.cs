using UnityEngine;

public class EndScreenHappiness : MonoBehaviour
{
    void Start()
    {
        print(Mathf.Floor(GameLogic.happiness));   
    }


}
