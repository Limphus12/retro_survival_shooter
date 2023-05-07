using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    public class AIManager : MonoBehaviour
    {
        private NavMeshAgent agent;
        public Vector3 OriginPosition { get; private set; }

        private Vector3 targetPosition, previousTargetPosition;

        public FieldOfView FOV { get; private set; }

        public bool IsMoving { get; private set; }

        public void SetTargetPos(Vector3 pos)
        {
            targetPosition = pos; SetDestination();
        }

        private void Awake() => Init();

        private void Init()
        {
            agent = GetComponent<NavMeshAgent>();
            FOV = GetComponent<FieldOfView>();

            OriginPosition = transform.position;
            targetPosition = transform.position;

            IsMoving = false;
        }

        private void Update()
        {
            CheckDestination();
        }

        private void CheckDestination()
        {
            if (Vector3.Distance(transform.position, targetPosition) < 2f)
            {
                IsMoving = false;
            }

            else IsMoving = true;
        }

        public void SetDestination()
        {
            if (targetPosition != previousTargetPosition)
            {
                agent.SetDestination(targetPosition);
            }

            previousTargetPosition = targetPosition;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(OriginPosition, 0.25f);
        }
    }
}