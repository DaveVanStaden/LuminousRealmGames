using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    // int isStrafingRightHash;
    // int isStrafingLeftHash;
    int isTrottingHash;
    int isIdleHash;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        isTrottingHash = Animator.StringToHash("isTrotting");
        isIdleHash = Animator.StringToHash("isIdle");
    }

    // Update is called once per frame
    void Update()
    {
        // bool isStrafingRight = animator.GetBool(isStrafingRightHash);
        // bool isStrafingLeft = animator.GetBool(isStrafingLeftHash);
        // bool rightPressed = Input.GetKey("d");
        //bool leftPressed = Input.GetKey("a");

        bool isTrotting = animator.GetBool(isTrottingHash);
        bool isIdle = animator.GetBool(isIdleHash);
        bool isMoving = Input.GetKey("a") || Input.GetKey("w") || Input.GetKey("d") || Input.GetKey("s");

        if (isMoving)
        {
            animator.SetBool(isTrottingHash, true);
            animator.SetBool(isIdleHash, false);
        } 
        else
        {
            animator.SetBool(isTrottingHash, false);
            animator.SetBool(isIdleHash, true);
        }


       // if (rightPressed) 
       // {
        //    animator.SetBool(isStrafingRightHash, true);
       // }

       // if (leftPressed)
        //{
       //     animator.SetBool(isStrafingLeftHash, true);
       // }

        //if (!rightPressed)
       // {
       //     animator.SetBool(isStrafingRightHash, false);
        //}


       // if (!leftPressed)
       // {
         //   animator.SetBool(isStrafingLeftHash, false);
       // }
    }
}
