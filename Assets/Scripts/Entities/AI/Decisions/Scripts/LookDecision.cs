using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(menuName = "AI/Decisions/Look")]
    public class LookDecision : Decision
    {
        public override bool Decide(AIManager ai)
        {
            return Look(ai);
        }

        private bool Look(AIManager ai)
        {
            FieldOfView fov = ai.GetComponent<FieldOfView>();

            if (!fov) return false;

            if (fov.closestTarget != null)
            {
                ai.SetTargetPos(fov.closestTarget.position);
                return true;
            }

            else return false;
        }
    }
}