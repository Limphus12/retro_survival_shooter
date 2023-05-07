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
        private AIAction currentState;

        private AIManager ai;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            // Get the AIManager component from this game object
            ai = GetComponent<AIManager>();

            // Set the initial state to the first action in the list
            currentState = actions[0];
        }

        private void Update()
        {
            UpdateState();
        }

        private void UpdateState()
        {
            // Execute the current state's action
            currentState.Act(ai);

            // Check if a transition to a new state is necessary
            foreach (Transition transition in currentState.transitions)
            {
                if (transition.CheckCondition(ai))
                {
                    SetState(transition.trueState); break;
                }

                else if (!transition.CheckCondition(ai))
                {
                    SetState(transition.falseState); break;
                }
            }
        }

        private void SetState(AIAction newState)
        {
            if (newState == currentState) return;
            else currentState = newState;
            Debug.Log(currentState);
        }
    }
}