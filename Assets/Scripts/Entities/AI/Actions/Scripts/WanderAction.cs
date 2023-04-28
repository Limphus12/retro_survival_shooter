using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(menuName = "AI/Actions/Wander")]
    public class WanderAction : Action
    {
        [SerializeField] private LayerMask layerMask;

        [Space]
        [SerializeField] private Vector2 wanderTimeRange;
        [SerializeField] private float wanderDistance;

        [Space]
        [SerializeField] private bool remainAroundOrigin;

        private float timer, currentWanderTime;

        public override void Act(AIManager ai)
        {
            CheckWander(ai);
        }

        private void CheckWander(AIManager ai)
        {
            timer += Time.deltaTime;

            if (timer >= currentWanderTime)
            {
                Vector3 targetPos;

                if (remainAroundOrigin) targetPos = RandomNavSphere(ai.OriginPosition, wanderDistance, layerMask);
                else targetPos = RandomNavSphere(ai.transform.position, wanderDistance, layerMask);

                ai.SetTargetPos(targetPos);

                ResetTimer();
            }
        }

        private void ResetTimer()
        {
            //pick a new random time, and reset the timer
            currentWanderTime = Random.Range(wanderTimeRange.x, wanderTimeRange.y); timer = 0;
        }

        //sourced from - https://forum.unity.com/threads/solved-random-wander-ai-using-navmesh.327950/
        public static Vector3 RandomNavSphere(Vector3 origin, float dist, LayerMask layerMask)
        {
            Vector3 randDirection = Random.insideUnitSphere * dist + origin;

            NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layerMask);

            return navHit.position;
        }
    }
}