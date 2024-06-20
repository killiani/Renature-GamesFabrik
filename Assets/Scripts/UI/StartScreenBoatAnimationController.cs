using System.Collections;
using UnityEngine;

public class BoatAnimationController : MonoBehaviour
{
    public GameObject rightBoat;
    public GameObject leftBoat;
    private Animator rightBoatAnimator;
    private Animator leftBoatAnimator;

    void Start()
    {
        rightBoatAnimator = rightBoat.GetComponent<Animator>();
        leftBoatAnimator = leftBoat.GetComponent<Animator>();
        StartCoroutine(BoatSequence());
    }

    private IEnumerator BoatSequence()
    {
        while (true)
        {
            rightBoatAnimator.SetTrigger("rightBoat");
            yield return new WaitUntil(() => rightBoatAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
            yield return new WaitForSeconds(17);

            leftBoatAnimator.SetTrigger("leftBoat");
            yield return new WaitUntil(() => leftBoatAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
            yield return new WaitForSeconds(30);
        }
    }
}
