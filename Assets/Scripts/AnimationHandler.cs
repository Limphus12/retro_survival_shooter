using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class AnimationHandler : MonoBehaviour
    {
        protected Animator animator;

        private void Awake()
        {
            if (!animator) animator = GetComponent<Animator>();
        }

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