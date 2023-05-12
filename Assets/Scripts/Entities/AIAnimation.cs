using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    public class AIAnimation : AnimationHandler
    {
        private AIManager ai;

        private void Awake() => Init();

        private void Init()
        {
            // Get the AIManager component from this game object
            ai = GetComponent<AIManager>();
        }

        private void Update()
        {
            if (ai.IsMoving) Move();
            else StopMove();

            SetSpeed();
        }

        public void StopAnimation()
        {
            animator.enabled = false;
        }

        const string MOVING = "IsMoving";
        const string SPEED = "Speed";

        void SetSpeed() => SetParamater(SPEED, ai.CurrentSpeed);

        public void Move() => SetParamater(MOVING, true);
        public void StopMove() => SetParamater(MOVING, false);
    }
}