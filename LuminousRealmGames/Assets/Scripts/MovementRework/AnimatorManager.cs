using System;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public Animator animator;
    private int horizontal;
    private int vertical;
    private static readonly int IsInteracting = Animator.StringToHash("isInteracting");

    private void Awake()
    {
        animator.GetComponent<Animator>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }

    public void PlayTargetAnimation(string targetAnimation, bool isInteracting)
    {
        animator.SetBool(IsInteracting, isInteracting);
        animator.CrossFade(targetAnimation,0.2f);
    }

    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting, PlayerHealth.PlayerState isInjured)
    {
        float snappedHorizontal;
        float snappedVertical;

        #region Snapped Horizontal
        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            snappedHorizontal = 0.5f;
        } else if (horizontalMovement > 0.55f)
        {
            snappedHorizontal = 1f;
        }else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
        {
            snappedHorizontal = -0.5f;
        } else if (horizontalMovement < -0.55f)
        {
            snappedHorizontal = -1;
        }
        else
        {
            snappedHorizontal = 0;
        }
        #endregion
        #region Snapped Vertical
        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            snappedVertical = 0.5f;
        } else if (verticalMovement > 0.55f)
        {
            snappedVertical = 1f;
        }else if (verticalMovement < 0 && verticalMovement > -0.55f)
        {
            snappedVertical = -0.5f;
        } else if (verticalMovement < -0.55f)
        {
            snappedVertical = -1;
        }
        else
        {
            snappedVertical = 0;
        }
        #endregion

        if (isSprinting)
        {
            snappedVertical = 2;
            snappedHorizontal = horizontalMovement;
        }
        if(isInjured == PlayerHealth.PlayerState.Injured && snappedVertical >= 0.5f)
        {
            snappedVertical = 0.5f;
            snappedHorizontal = horizontalMovement;
        } else if (isInjured == PlayerHealth.PlayerState.Injured && snappedVertical == 0)
        {
            snappedVertical = 0;
            snappedHorizontal = horizontalMovement;
        }

        animator.SetFloat(horizontal,snappedHorizontal,0.1f,Time.deltaTime);
        animator.SetFloat(vertical,snappedVertical,0.1f,Time.deltaTime);
    }
}
