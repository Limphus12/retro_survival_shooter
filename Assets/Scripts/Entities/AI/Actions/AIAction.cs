using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public abstract class AIAction : MonoBehaviour
    {
        // List of possible transitions to other AI actions
        [Tooltip("List of possible transitions to other AI actions - Transitions at the top of list will be priortized first.")]
        public List<Transition> transitions = new List<Transition>();

        // Execute the action for this state
        public abstract void Act(AIManager ai);

        //perhaps add this as a way to reset the state when switching to it (like for the search state)?
        //public abstract void ResetState();
    }
}