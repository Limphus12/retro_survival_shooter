using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    public enum PatrolMode { Loop, PingPong, Random }

    public class AIPatrol : AIAction
    {
        [SerializeField] private PatrolMode patrolMode;

        [Space, SerializeField] private Transform[] patrolPoints;

        [Space]
        [SerializeField] private Vector2 patrolWaitTimeRange;

        private float waitTimer, currentWaitTime;

        private int currentPatrolPoint;

        private bool pingPong;

        public override void Act(AIManager ai)
        {
            CheckPatrol(ai);

            ai.Walk();
        }

        private void CheckPatrol(AIManager ai)
        {
            if (ai.IsMoving) return;

            else if (!ai.IsMoving)
            {
                waitTimer += Time.deltaTime;

                if (waitTimer >= currentWaitTime)
                {
                    ResetIdleTimer(); SelectPatrolPoint(ai);
                }
            }
        }

        private void SelectPatrolPoint(AIManager ai)
        {
            if (patrolPoints.Length == 0) return;

            switch (patrolMode)
            {
                case PatrolMode.Loop:

                    currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;

                    break;
                case PatrolMode.PingPong:

                    if (currentPatrolPoint >= patrolPoints.Length - 1)
                    {
                        pingPong = true;
                    }

                    else if (currentPatrolPoint <= 0)
                    {
                        pingPong = false;
                    }

                    if (pingPong) currentPatrolPoint--;
                    else currentPatrolPoint++;

                    break;
                case PatrolMode.Random:

                    currentPatrolPoint = Random.Range(0, patrolPoints.Length);

                    break;
            }

            Vector3 targetPoint = AINavigation.WorldToNavPoint(patrolPoints[currentPatrolPoint].position,LayerMask.NameToLayer("Terrain"));

            ai.SetTargetPos(targetPoint);
        }

        private void ResetIdleTimer()
        {
            currentWaitTime = Random.Range(patrolWaitTimeRange.x, patrolWaitTimeRange.y); waitTimer = 0;
        }
    }
}