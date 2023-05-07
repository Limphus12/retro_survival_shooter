using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(menuName = "Conditions/HasFOVTarget")]
    public class HasFOVTarget : Condition
    {
        public override bool CheckCondition(AIManager ai)
        {
            return ai.FOV.VisibleTargets.Count > 0;
        }
    }
}