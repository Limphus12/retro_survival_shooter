using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    public enum TargetPreference { First, Closest, Random }

    public class AIChase : AIAction
    {
        [SerializeField] private TargetPreference targetPreference;

        [SerializeField] private FieldOfView fov;

        public bool HasTarget { get; private set; }

        public override void Act(AIManager ai)
        {
            CheckChase(ai);
        }

        public override bool Condition(AIManager ai)
        {
            return !HasTarget;
        }

        private void CheckChase(AIManager ai)
        {
            if (!fov) return;

            else if (fov.VisibleTargets != null)
            {
                if (fov.VisibleTargets.Count == 0) HasTarget = false;

                else Chase(ai);
            }
        }

        private void Chase(AIManager ai)
        {
            HasTarget = true;

            Vector3 targetPos = Vector3.zero;

            switch (targetPreference)
            {
                case TargetPreference.First:

                    targetPos = fov.VisibleTargets[0].position;

                    break;

                case TargetPreference.Closest:

                    targetPos = fov.GetClosestTarget().position;

                    break;

                case TargetPreference.Random:

                    targetPos = fov.VisibleTargets[Random.Range(0, fov.VisibleTargets.Count)].position;

                    break;
            }

            ai.SetTargetPos(targetPos);
        }
    }
}