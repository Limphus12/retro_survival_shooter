using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    public class AISearch : AIAction
    {
        [SerializeField] private LayerMask layerMask;

        [Space]
        [SerializeField] private Vector2 searchTimeRange, searchWaitTimeRange;
        [SerializeField] private float wanderDistance;

        private float searchTimer = 0, waitTimer = 0, currentSearchTime, currentWaitTime;

        public bool IsSearching { get; private set; }
        public bool HasSearched{ get; private set; }

        public override void Act(AIManager ai)
        {
            Search(ai);
        }

        public void StartSearch()
        {
            ResetSearchTimer();

            IsSearching = true;
            HasSearched = false;
        }

        private void EndSearch()
        {
            searchTimer = 0;

            IsSearching = false;
            HasSearched = true;
        }

        private void ResetSearch()
        {
            searchTimer = 0;

            IsSearching = false;
            HasSearched = false;
        }

        private void Search(AIManager ai)
        {
            if (searchTimer == 0) StartSearch();

            searchTimer += Time.deltaTime;
            
            if (searchTimer > currentSearchTime) { EndSearch(); return; }

            if (ai.IsMoving) return;

            else if (!ai.IsMoving)
            {
                waitTimer += Time.deltaTime;

                if (waitTimer >= currentWaitTime)
                {
                    ai.SetTargetPos(AINavigation.RandomNavSphere(ai.transform.position, wanderDistance, layerMask));

                    ResetIdleTimer();
                }
            }
        }

        private void ResetIdleTimer()
        {
            currentWaitTime = Random.Range(searchWaitTimeRange.x, searchWaitTimeRange.y); waitTimer = 0;
        }

        private void ResetSearchTimer()
        {
            currentSearchTime = Random.Range(searchTimeRange.x, searchTimeRange.y); searchTimer = 0;
        }
    }
}