using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class AnimationHandler : MonoBehaviour
    {
        [Header("Attributes - Animation")]
        [SerializeField] protected Animator animator;

        protected string currentState;

        protected void PlayAnimation(string newState)
        {
            //stops the same animation from interrupting itself.
            if (currentState == newState) return;

            //play the animation
            animator.Play(newState);

            //reassign the current state
            currentState = newState;
        }
    }
}