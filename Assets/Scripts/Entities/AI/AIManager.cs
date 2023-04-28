using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace com.limphus.retro_survival_shooter
{
    public class AIManager : MonoBehaviour
    {
        [SerializeField] private State currentState;
        [SerializeField] private State remainState;

        private float stateTimeElapsed;

        private NavMeshAgent agent;
        public Vector3 OriginPosition { get; private set; }
        public Vector3 LastKnownTargetPosition { get; private set; }

        private Vector3 targetPosition, previousTargetPosition;

        public void SetTargetPos(Vector3 pos) => targetPosition = pos;
        public void SetLastKnownTargetPosition(Vector3 pos) => LastKnownTargetPosition = pos;

        private void Awake() => Init();

        private void Init()
        {
            agent = GetComponent<NavMeshAgent>();
            OriginPosition = transform.position;
        }

        private void Update()
        {
            CalculateState();
        }

        private void CalculateState()
        {
            currentState.UpdateState(this);
                
            SetDestination();
        }

        public void TransitionToState(State nextState)
        {
            if (nextState != remainState)
            {
                currentState = nextState;
                ExitState();
            }
        }

        public bool HasTimeElapsed(float duration)
        {
            stateTimeElapsed += Time.deltaTime;

            if (stateTimeElapsed >= duration)
            {
                stateTimeElapsed = 0;
                return true;
            }

            else return false;
        }

        private void ExitState()
        {
            stateTimeElapsed = 0;
        }

        private void SetDestination()
        {
            if (targetPosition != previousTargetPosition)
            {
                agent.SetDestination(targetPosition);
            }

            previousTargetPosition = targetPosition;
        }

        private void OnDrawGizmos()
        {
            if (currentState)
            {
                Gizmos.color = currentState.gizmosColor;
                Gizmos.DrawWireSphere(transform.position + Vector3.up, 2f);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(OriginPosition, 0.5f);
        }
    }
}