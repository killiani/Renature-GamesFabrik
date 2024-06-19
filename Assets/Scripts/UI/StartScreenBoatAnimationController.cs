using UnityEngine;

public class BoatAnimationController : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        // Start the first animation
        animator.SetBool("StartBoot1", true);
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Boot1Animation") && stateInfo.normalizedTime >= 1.0f)
        {
            // Boot 1 Animation finished, start Boot 2 Animation
            animator.SetBool("StartBoot1", false);
            animator.SetBool("StartBoot2", true);
        }
        else if (stateInfo.IsName("Boot2Animation") && stateInfo.normalizedTime >= 1.0f)
        {
            // Boot 2 Animation finished, start Boot 1 Animation
            animator.SetBool("StartBoot2", false);
            animator.SetBool("StartBoot1", true);
        }
    }
}
