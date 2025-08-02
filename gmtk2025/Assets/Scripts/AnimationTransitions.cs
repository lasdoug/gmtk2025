using UnityEngine;

public class AnimationTransitions : MonoBehaviour
{
    GameLogic gameLogic;
    Animator animator;
    int age;
    bool isBaby, isAdult, isSenior;
    [SerializeField] int kidUpperBound, AdultUpperBound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        gameLogic = GameObject.FindGameObjectWithTag("Logic").GetComponent<GameLogic>();

    }

    // Update is called once per frame
    void Update()
    {
        age = gameLogic.year;

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
