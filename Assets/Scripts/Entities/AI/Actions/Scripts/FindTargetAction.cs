using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(menuName = "AI/Actions/FindTarget")]
    public class FindTargetAction : Action
    {
        public override void Act(AIManager ai)
        {
            FindTarget(ai);
        }

        private void FindTarget(AIManager ai)
        {
            //rotate 360 degrees
        }
    }
}