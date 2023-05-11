using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class AIStats : EntityStats
    {
        private RagdollManager ragdoll;
        private AIManager ai;
        private AIAnimation aiAnimation;

        bool death = false;

        [SerializeField] protected float walkSpeed, runSpeed;

        public float GetWalkSpeed() => walkSpeed;
        public float GetRunSpeed() => runSpeed;

        private void Start()
        {
            if (!ragdoll) ragdoll = GetComponentInChildren<RagdollManager>();
            if (!ai) ai = GetComponent<AIManager>();
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
                }

                if (aiAnimation) aiAnimation.StopAnimation();

                if (ragdoll) ragdoll.ToggleRagdoll(IsDead);
            }
        }
    }
}