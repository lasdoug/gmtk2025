using UnityEngine;
using UnityEngine.UI;

public class AnimationTransitions : MonoBehaviour
{
    Animator animator;
    int age;
    [SerializeField] int kidUpperBound, AdultUpperBound;
    [SerializeField] Color kidBkgColor, adultBkgColor, seniorBkgColor;
    Graphic guyBkg;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        guyBkg = GameObject.FindGameObjectWithTag("guyBkg").GetComponent<Graphic>();
    }

    // Update is called once per frame
    void Update()
    {
        age = GameLogic.year;

        if (age < kidUpperBound)
        {
            //run kid animation logic
            animator.SetBool("isBaby", true);
            SetBkgColour(kidBkgColor);
        }
        else if (age >= kidUpperBound && age < AdultUpperBound)
        {
            //run adult animation logic
            animator.SetBool("isBaby", false);
            animator.SetBool("isAdult", true);
            SetBkgColour(adultBkgColor);
        }
        else
        {
            //run elder animation logic
            animator.SetBool("isAdult", false);
            animator.SetBool("isSenior", true);
            SetBkgColour(seniorBkgColor);
        }
    }

    void SetBkgColour(Color c)
    {
        guyBkg.color = c;
    }
}
