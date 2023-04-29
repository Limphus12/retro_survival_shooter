using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public abstract class AIAction : MonoBehaviour
    {
        // List of possible transitions to other AI actions
        public List<AIAction> transitions = new List<AIAction>();

        // Get the list of possible transitions
        public List<AIAction> GetTransitions() => transitions;

        // Check if the condition for transitioning to this state is met
        public abstract bool Condition(AIManager ai);

        // Execute the action for this state
        public abstract void Act(AIManager ai);
    }
}