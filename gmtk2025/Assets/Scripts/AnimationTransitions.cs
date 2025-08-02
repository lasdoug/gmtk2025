using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class AnimationTransitions : MonoBehaviour
{
    Animator animator;
    int age;
    [SerializeField] int kidUpperBound, AdultUpperBound;
    Image guyBkg;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        age = GameLogic.year;

        if (age < kidUpperBound)
        {
            //run kid animation logic
            animator.SetBool("isBaby", true);
        }
        else if (age >= kidUpperBound && age < AdultUpperBound)
        {
            //run adult animation logic
            animator.SetBool("isBaby", false);
            animator.SetBool("isAdult", true);
        }
        else
        {
            //run elder animation logic
            animator.SetBool("isAdult", false);
            animator.SetBool("isSenior", true);
        }
    }
}
