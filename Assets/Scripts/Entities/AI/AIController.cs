using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    public enum AIStates { IDLE, WANDER, PATROL, CHASE, SEARCH, ATTACK }

    public enum AIIdleState { IDLE, WANDER, PATROL }
    public enum AIAlertState { CHASE, SEARCH, ATTACK }

    public class AIController : MonoBehaviour
    {
        [SerializeField] protected AIStates currentState;
        [SerializeField] protected AIIdleState defaultIdleState;

        public NavMeshAgent Agent { get; private set; }
        private Vector3 originPosition;

        private FieldOfView fov;
        public bool CanMove { get; set; }
        public bool IsMoving { get; private set; }
        public float CurrentSpeed => Agent.speed;

        private float targetDistance;
        private Vector3 targetPosition, previousTargetPosition;

        private AIStats stats;

        public void Stop() => Agent.speed = 0;

        public void WalkSpeed() => Agent.speed = stats.GetWalkSpeed();

        public void RunSpeed() => Agent.speed = stats.GetRunSpeed();
        public void SearchSpeed() => Agent.speed = stats.GetSearchSpeed();

        public void SetTargetPos(Vector3 pos)
        {
            targetPosition = pos; SetDestination();
        }

        private void Awake() => Init();

        private void Init()
        {
            Agent = GetComponent<NavMeshAgent>();
            fov = GetComponent<FieldOfView>();
            stats = GetComponent<AIStats>();

            originPosition = transform.position;
            targetPosition = transform.position;

            IsMoving = false;
            CanMove = true;

            WalkSpeed();

            if (firearm) firearm.FirePoint = firePoint;
        }

        private void Update()
        {
            CheckVision();
            CheckState();
            CheckDestination();
        }

        private void CheckState()
        {
            switch (currentState)
            {
                case AIStates.IDLE:

                    Idle();

                    break;

                case AIStates.WANDER:

                    CheckWander();

                    break;

                case AIStates.PATROL:

                    //insert patrol stuff later

                    break;

                case AIStates.CHASE:

                    CheckChase();

                    break;

                case AIStates.SEARCH:

                    if (!IsSearching) StartSearch();

                    Search();

                    break;

                case AIStates.ATTACK:

                    Attack();

                    break;
            }
        }

        private void CheckVision()
        {
            if (!fov) return;

            if (fov.VisibleTargets != null)
            {
                if (fov.VisibleTargets.Count == 0) hasTarget = false;

                else if (fov.VisibleTargets.Count > 0) hasTarget = true;
            }

            else if (fov.VisibleTargets == null) hasTarget = false;
        }

        private void Idle()
        {
            SetTargetPos(AINavigation.WorldToNavPoint(transform.position, -1));
        }

        #region Wander

        [Header("Attributes - Wander")]
        [SerializeField] private LayerMask wanderLayerMask;

        [Space]
        [SerializeField] private Vector2 wanderWaitTimeRange;
        [SerializeField] private float wanderDistance;

        [Space]
        [SerializeField] private bool remainAroundOrigin;

        private float wanderWaitTimer, currentWanderWaitTime;

        private void CheckWander()
        {
            if (hasTarget) currentState = AIStates.CHASE;

            WalkSpeed();

            if (IsMoving) return;

            else if (!IsMoving)
            {
                Stop();

                wanderWaitTimer += Time.deltaTime;

                if (wanderWaitTimer >= currentWanderWaitTime)
                {
                    ResetWander();
                }
            }
        }

        private void ResetWander()
        {
            //pick a new targetPos, and set it
            Vector3 targetPos;

            if (remainAroundOrigin) targetPos = AINavigation.RandomNavSphere(originPosition, wanderDistance, wanderLayerMask);
            else targetPos = AINavigation.RandomNavSphere(transform.position, wanderDistance, wanderLayerMask);

            SetTargetPos(targetPos);

            //pick a new random time, and reset the timer
            currentWanderWaitTime = Random.Range(wanderWaitTimeRange.x, wanderWaitTimeRange.y); wanderWaitTimer = 0;
        }

        #endregion

        #region Chase

        private bool hasTarget;

        private void CheckChase()
        {
            if (targetDistance <= attackDistance && hasTarget)
            {
                currentState = AIStates.ATTACK;
            }

            else if (!hasTarget) currentState = AIStates.SEARCH;

            else Chase();
        }

        private void Chase()
        {
            RunSpeed();

            Vector3 targetPos = fov.GetClosestTarget().position;

            SetTargetPos(targetPos);
        }

        #endregion

        #region Search

        [Header("Attributes - Search")]
        [SerializeField] private LayerMask searchLayerMask;

        [Space]
        [SerializeField] private Vector2 searchTimeRange, searchWaitTimeRange;
        [SerializeField] private float searchDistance;

        private float searchTimer = 0, searchWaitTimer = 0, currentSearchTime, currentSearchWaitTime;

        public bool IsSearching { get; private set; }

        private void StartSearch()
        {
            IsSearching = true;
        }

        private void Search()
        {
            if (targetDistance <= attackDistance && hasTarget)
            {
                currentState = AIStates.ATTACK;
            }

            else if (!hasTarget) currentState = AIStates.SEARCH;

            SearchSpeed();

            if (currentSearchTime == 0) ResetSearchTimer();

            searchTimer += Time.deltaTime;

            if (searchTimer > currentSearchTime) EndSearch();

            if (IsMoving) return;

            else if (!IsMoving)
            {
                searchWaitTimer += Time.deltaTime;

                if (searchWaitTimer >= currentSearchWaitTime)
                {
                    ResetSearch();
                }
            }
        }

        private void ResetSearch()
        {
            SetTargetPos(AINavigation.RandomNavSphere(transform.position, searchDistance, searchLayerMask));
            currentSearchWaitTime = Random.Range(searchWaitTimeRange.x, searchWaitTimeRange.y); searchWaitTimer = 0;
        }

        private void ResetSearchTimer()
        {
            currentSearchTime = Random.Range(searchTimeRange.x, searchTimeRange.y); searchTimer = 0;
        }

        private void EndSearch()
        {
            IsSearching = false;

            currentState = (AIStates)defaultIdleState;
        }

        #endregion

        #region Attack

        [Header("Attributes - Attacking")]
        [SerializeField] private Firearm firearm;

        [Space, SerializeField] private float attackDistance;

        [Space, SerializeField] private Transform firePoint;

        private void Attack()
        {
            if (targetDistance > attackDistance && hasTarget) { currentState = AIStates.CHASE; IsAttacking = false; IsSearching = false; return; }

            else if (!hasTarget) { currentState = AIStates.SEARCH; IsAttacking = false; return; }

            IsAttacking = true;
            IsSearching = false;

            Stop(); SetTargetPos(transform.position);

            // The step size is equal to speed times frame time.
            float singleStep = 15 * Time.deltaTime;
            
            Vector3 targetDirection = transform.position + Vector3.forward;

            if (fov.GetClosestTarget() != null)
            {
                targetDirection = fov.GetClosestTarget().position - transform.position;
            }

            targetDirection.y = 0;

            // Rotate the forward vector towards the target direction by one step
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            
            // Calculate a rotation a step closer to the target and applies rotation to this object
            transform.rotation = Quaternion.LookRotation(newDirection);

            UpdateFirePoint();

            if (firearm && !firearm.InUse())
            {
                firearm.CheckInputs(true, false, false);
            }
        }

        private void UpdateFirePoint()
        {
            if (fov && firePoint) firePoint.transform.LookAt(fov.GetClosestTarget().position);
        }

        public bool IsFiring => firearm.IsAttacking;
        public bool IsAttacking { get; private set; }

        #endregion

        private void CheckDestination()
        {
            if (fov.GetClosestTarget() != null) targetDistance = Vector3.Distance(transform.position, fov.GetClosestTarget().position);

            if (Vector3.Distance(transform.position, targetPosition) < 1f)
            {
                IsMoving = false;

                Stop();
            }

            else IsMoving = true;
        }

        public void SetDestination()
        {
            if (targetPosition != previousTargetPosition)
            {
                Agent.SetDestination(targetPosition);
            }

            previousTargetPosition = targetPosition;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(originPosition, 0.2f);

            Gizmos.color = Color.red;
            if (Agent) Gizmos.DrawSphere(Agent.destination, 0.2f);
        }
    }
}