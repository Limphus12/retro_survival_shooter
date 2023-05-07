using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [System.Serializable]
    public class Transition
    {
        // The condition for this transition to be taken
        public Condition condition;

        // The states to transition to if the condition is true/false
        public AIAction trueState, falseState;

        // Check if the condition is true for the given AIManager
        public bool CheckCondition(AIManager ai)
        {
            // Check if the condition is true
            return condition.CheckCondition(ai);
        }
    }
}