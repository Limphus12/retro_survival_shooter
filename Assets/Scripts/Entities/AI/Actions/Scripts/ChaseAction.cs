using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(menuName = "AI/Actions/Chase")]
    public class ChaseAction : Action
    {
        public override void Act(AIManager ai)
        {
            Chase(ai);
        }

        private void Chase(AIManager ai)
        {
            FieldOfView fov = ai.GetComponent<FieldOfView>();

            if (!fov) return;

            if (fov.closestTarget != null)
            {
                ai.SetTargetPos(fov.closestTarget.position);
                ai.SetLastKnownTargetPosition(fov.closestTarget.position);
            }

            else
            {
                ai.SetTargetPos(ai.LastKnownTargetPosition);
            }
        }
    }
}