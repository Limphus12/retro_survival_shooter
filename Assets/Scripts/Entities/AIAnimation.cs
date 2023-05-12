using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    public class AIAnimation : AnimationHandler
    {
        private AIController ai;

        private void Awake() => Init();

        private void Init()
        {
            // Get the AIManager component from this game object
            ai = GetComponent<AIController>();
        }

        private void Update()
        {
            UpdateAnimations();
        }

        private void UpdateAnimations()
        {
            SetSpeed();
            SetMove();

            SetSearch();
            SetAttacking();
            SetFiring();
        }

        public void ToggleAnimator(bool b)
        {
            animator.enabled = b;
        }

        const string MOVING = "IsMoving";
        const string SPEED = "Speed";
        const string SEARCH = "IsSearching";
        const string ATTACK = "IsAttacking";
        const string FIRING = "IsFiring";
        const string RELOAD = "IsReloading";

        private void SetSpeed() => SetParamater(SPEED, ai.CurrentSpeed);
        private void SetMove() => SetParamater(MOVING, ai.IsMoving);
        private void SetSearch() => SetParamater(SEARCH, ai.IsSearching);
        private void SetAttacking() => SetParamater(ATTACK, ai.IsFiring);
        private void SetFiring() => SetParamater(FIRING, ai.IsFiring);
        //private void SetReloading() => SetParamater(RELOAD, ai.IsReloading);
    }
}