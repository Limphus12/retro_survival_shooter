using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;
using System;

namespace com.limphus.retro_survival_shooter
{
    public enum State
    {
        Idle, Wander, Patrol, Search, Chase, Attack
    }
    
    public class AIState : MonoBehaviour
    {
        // List of possible AI actions
        public List<AIAction> actions = new List<AIAction>();

        // Current state of the AI
        private AIAction currentAction;

        // Dictionary of all possible transitions and the states they lead to
        private Dictionary<AIAction, List<AIAction>> transitions = new Dictionary<AIAction, List<AIAction>>();

        private AIManager ai;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            ai = GetComponent<AIManager>();

            // Set up the transitions for each AI action
            foreach (AIAction action in actions)
            {
                transitions[action] = action.GetTransitions();
            }

            // Set the initial state to the first action in the list
            currentAction = actions[0];
        }

        private void Update()
        {
            UpdateState();
        }

        private void UpdateState()
        {
            // Execute the current state's action
            currentAction.Act(ai);

            // Check if a transition to a new state is necessary
            foreach (AIAction action in transitions[currentAction])
            {
                if (action.Condition(ai))
                {
                    // Transition to the new state
                    currentAction = action;

                    Debug.Log(currentAction);

                    break;
                }
            }
        }
    }
}