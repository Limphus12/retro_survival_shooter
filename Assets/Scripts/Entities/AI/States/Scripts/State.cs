using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(menuName = "AI/State")]
    public class State : ScriptableObject
    {
        public Action[] actions;
        public Transition[] transitions;
        public Color gizmosColor = Color.white;

        public void UpdateState(AIManager ai)
        {
            ExecuteActions(ai);
            CheckForTransitions(ai);
        }

        private void ExecuteActions(AIManager ai)
        {
            foreach (var action in actions)
            {
                action.Act(ai);
            }
        }

        private void CheckForTransitions(AIManager ai)
        {
            foreach (var transition in transitions)
            {
                bool decision = transition.decision.Decide(ai);

                if (decision) ai.TransitionToState(transition.trueState);
                else ai.TransitionToState(transition.falseState);
            }
        }
    }
}