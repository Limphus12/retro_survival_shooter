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
        private AIManager ai;

        [SerializeField] private State currentState;

        [Header("AI - Movement")]
        [SerializeField] private AIWander wander;
        [SerializeField] private AIChase chase;
        [SerializeField] private AISearch search;
        [SerializeField] private AIPatrol patrol;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            ai = GetComponent<AIManager>();

            wander = GetComponent<AIWander>();
            chase = GetComponent<AIChase>();
            search = GetComponent<AISearch>();
            patrol = GetComponent<AIPatrol>();
        }

        private void Update()
        {
            CheckState();
        }

        private void CheckState()
        {
            switch (currentState)
            {
                case State.Idle:

                    Debug.Log("Idle");

                    break;

                case State.Wander:

                    if (wander) wander.Act(ai);
                    else Debug.Log("Cannot Wander");

                    break;

                case State.Patrol:

                    if (patrol) patrol.Act(ai);
                    else Debug.Log("Cannot Patrol");

                    Debug.Log("Patrolling");

                    break;

                case State.Search:

                    if (search)
                    {
                        if (!search.IsSearching && !search.HasSearched) search.StartSearch();

                        if (search.IsSearching) { search.Act(ai); Debug.Log("Searching"); }

                        else if (search.HasSearched) Debug.Log("Has Searched!");
                    }

                    else Debug.Log("Cannot Search");

                    break;

                case State.Chase:

                    if (chase) chase.Act(ai);
                    else Debug.Log("Cannot Chase");

                    break;

                case State.Attack:

                    Debug.Log("Attacking");



                    break;

                default:
                    break;
            }
        }


    }
}