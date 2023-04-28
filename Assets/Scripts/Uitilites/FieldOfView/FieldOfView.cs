using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.utilities
{
    public class FieldOfView : MonoBehaviour
    {
        [SerializeField] private string targetTag;

        public float viewRadius; [Range(0, 360)] public float viewAngle;

        [HideInInspector]
        public List<Transform> visibleTargets = new List<Transform>();
        public Transform closestTarget;

        private void Update()
        {
            FindVisibleTargets();
        }

        void FindVisibleTargets()
        {
            visibleTargets.Clear();
            closestTarget = null;

            Collider[] targets = Physics.OverlapSphere(transform.position, viewRadius, -1);

            float prevDistance = -100; //-100 as the distance can never be beneath 0 (used for init)

            for (int i = 0; i < targets.Length; i++)
            {
                Transform target = targets[i].transform;

                if (target.tag != targetTag) continue;

                Vector3 dirToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float disToTarget = Vector3.Distance(transform.position, target.position);

                    if (prevDistance == -100) prevDistance = disToTarget; 

                    //set our closest target
                    if (disToTarget <= prevDistance) closestTarget = target;

                    if (Physics.Raycast(transform.position, dirToTarget, out RaycastHit hit, disToTarget))
                    {
                        if (hit.transform == target) visibleTargets.Add(target);
                    }
                }
            }
        }

        public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}