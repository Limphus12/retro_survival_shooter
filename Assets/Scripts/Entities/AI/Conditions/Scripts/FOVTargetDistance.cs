using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(menuName = "Conditions/Distance")]
    public class FOVTargetDistance : Condition
    {
        public float distance;

        public override bool CheckCondition(AIManager ai)
        {
            if (ai.FOV.GetClosestTarget()) return Vector3.Distance(ai.FOV.GetClosestTarget().position, ai.transform.position) <= distance;
            else return false;
        }
    }
}