using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class AIStats : EntityStats
    {
        private RagdollManager ragdoll;
        private AIController ai;
        private AIAnimation aiAnimation;

        bool death = false;

        [SerializeField] protected float walkSpeed, runSpeed, searchSpeed;

        public float GetWalkSpeed() => walkSpeed;
        public float GetRunSpeed() => runSpeed;
        public float GetSearchSpeed() => searchSpeed;

        private void Start()
        {
            if (!ragdoll) ragdoll = GetComponentInChildren<RagdollManager>();
            if (!ai) ai = GetComponent<AIController>();
            if (!aiAnimation) aiAnimation = GetComponentInChildren<AIAnimation>();
            ragdoll.Stats = this;
        }

        protected override void Kill()
        {
            if (!death)
            {
                death = true;

                base.Kill();

                if (ai)
                {
                    ai.SetTargetPos(transform.position);
                    ai.Agent.enabled = false;
                    ai.CanMove = false;
                    ai.enabled = false;
                }

                if (aiAnimation) aiAnimation.ToggleAnimator(false);

                if (ragdoll) ragdoll.ToggleRagdoll(IsDead);
            }
        }
    }
}