using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    public class AIWander : AIAction
    {
        [SerializeField] private LayerMask layerMask;

        [Space]
        [SerializeField] private Vector2 wanderWaitTimeRange;
        [SerializeField] private float wanderDistance;

        [Space]
        [SerializeField] private bool remainAroundOrigin;

        private float waitTimer, currentWaitTime;

        public override void Act(AIManager ai)
        {
            CheckWander(ai);
        }

        private void CheckWander(AIManager ai)
        {
            if (ai.IsMoving) return;

            else if (!ai.IsMoving)
            {
                waitTimer += Time.deltaTime;

                if (waitTimer >= currentWaitTime)
                {
                    Vector3 targetPos;

                    if (remainAroundOrigin) targetPos = AINavigation.RandomNavSphere(ai.OriginPosition, wanderDistance, layerMask);
                    else targetPos = AINavigation.RandomNavSphere(ai.transform.position, wanderDistance, layerMask);

                    ai.SetTargetPos(targetPos);

                    ResetTimer();
                }
            }
        }

        private void ResetTimer()
        {
            //pick a new random time, and reset the timer
            currentWaitTime = Random.Range(wanderWaitTimeRange.x, wanderWaitTimeRange.y); waitTimer = 0;
        }
    }
}