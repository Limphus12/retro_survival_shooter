using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(menuName = "AI/Decisions/TargetLost")]
    public class TargetLostDecision : Decision
    {
        public float searchTime;

        public override bool Decide(AIManager ai)
        {
            return IsTargetVisible(ai);
        }

        private bool IsTargetVisible(AIManager ai)
        {
            return ai.HasTimeElapsed(searchTime);
        }
    }
}