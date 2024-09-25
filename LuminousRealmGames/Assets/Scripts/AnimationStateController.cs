using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    int isStrafingRightHash;
    int isStrafingLeftHash;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        isStrafingRightHash = Animator.StringToHash("isStrafingRight");
        isStrafingLeftHash = Animator.StringToHash("isStrafingLeft");

    }

    // Update is called once per frame
    void Update()
    {
        bool isStrafingRight = animator.GetBool(isStrafingRightHash);
        bool isStrafingLeft = animator.GetBool(isStrafingLeftHash);
        bool rightPressed = Input.GetKey("d");
        bool leftPressed = Input.GetKey("a");

        if (rightPressed) 
        {
            animator.SetBool(isStrafingRightHash, true);
        }

        if (leftPressed)
        {
            animator.SetBool(isStrafingLeftHash, true);
        }

        if (!rightPressed)
        {
            animator.SetBool(isStrafingRightHash, false);
        }


        if (!leftPressed)
        {
            animator.SetBool(isStrafingLeftHash, false);
        }
    }
}
