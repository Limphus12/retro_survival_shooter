using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    public enum AIStateEnum { IDLE, WANDER, PATROL, CHASE, SEARCH, ATTACK }

    public class AIController : MonoBehaviour
    {
        [SerializeField] protected AIStateEnum currentState;

        private NavMeshAgent agent;
        private Vector3 originPosition;

        private FieldOfView fov;
        public bool CanMove { get; set; }
        public bool IsMoving { get; private set; }
        public float CurrentSpeed => agent.speed;

        private Vector3 targetPosition, previousTargetPosition;

        private AIStats stats;

        public void Stop() => agent.speed = 0;

        public void Walk() => agent.speed = stats.GetWalkSpeed();

        public void Run() => agent.speed = stats.GetRunSpeed();

        public void SetTargetPos(Vector3 pos)
        {
            targetPosition = pos; SetDestination();
        }

        private void Awake() => Init();

        private void Init()
        {
            agent = GetComponent<NavMeshAgent>();
            fov = GetComponent<FieldOfView>();
            stats = GetComponent<AIStats>();

            originPosition = transform.position;
            targetPosition = transform.position;

            IsMoving = false;
            CanMove = true;

            Walk();
        }

        private void Update()
        {
            CheckState();
            CheckVision();
            CheckDestination();
        }

        private void CheckState()
        {
            switch (currentState)
            {
                case AIStateEnum.IDLE:

                    SetTargetPos(AINavigation.WorldToNavPoint(transform.position, -1));

                    break;

                case AIStateEnum.WANDER:

                    CheckWander();

                    break;

                case AIStateEnum.PATROL:

                    

                    break;

                case AIStateEnum.CHASE:

                    CheckChase();

                    break;

                case AIStateEnum.SEARCH:

                    Walk();

                    break;

                case AIStateEnum.ATTACK:

                    Stop();

                    break;

                default:
                    break;
            }
        }

        private void CheckVision()
        {
            if (!fov) return;

            if (fov.VisibleTargets != null)
            {
                if (fov.VisibleTargets.Count == 0) hasTarget = false;

                else hasTarget = true;
            }

            if (fov.VisibleTargets == null) hasTarget = false;
        }

        #region Wander

        [Header("Attributes - Wander")]
        [SerializeField] private LayerMask layerMask;

        [Space]
        [SerializeField] private Vector2 wanderWaitTimeRange;
        [SerializeField] private float wanderDistance;

        [Space]
        [SerializeField] private bool remainAroundOrigin;

        private float waitTimer, currentWaitTime;

        private void CheckWander()
        {
            Walk();

            if (IsMoving) return;

            else if (!IsMoving)
            {
                Stop();

                waitTimer += Time.deltaTime;

                if (waitTimer >= currentWaitTime)
                {
                    ResetWander();
                }
            }
        }

        private void ResetWander()
        {
            //pick a new targetPos, and set it
            Vector3 targetPos;

            if (remainAroundOrigin) targetPos = AINavigation.RandomNavSphere(originPosition, wanderDistance, layerMask);
            else targetPos = AINavigation.RandomNavSphere(transform.position, wanderDistance, layerMask);

            SetTargetPos(targetPos);

            //pick a new random time, and reset the timer
            currentWaitTime = Random.Range(wanderWaitTimeRange.x, wanderWaitTimeRange.y); waitTimer = 0;
        }

        #endregion

        #region Chase

        private bool hasTarget;

        private void CheckChase()
        {
            if (!hasTarget) return;

            else Chase();
        }

        private void Chase()
        {
            Run();

            Vector3 targetPos = fov.GetClosestTarget().position;

            SetTargetPos(targetPos);
        }

        #endregion



        private void CheckDestination()
        {
            if (Vector3.Distance(transform.position, targetPosition) < 0.25f)
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
            Gizmos.DrawWireSphere(originPosition, 0.25f);
        }
    }
}