using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.utilities
{
    public class FieldOfView : MonoBehaviour
    {
        [SerializeField] private string targetTag;

        [Space] public Transform eyes;

        public float viewRadius; public float minViewRadius;
        [Range(0, 360)] public float viewAngle;

        public List<Transform> VisibleTargets { get; private set; }

        public Transform GetClosestTarget()
        {
            Transform closestTarget = null;

            float distance = Mathf.Infinity;

            foreach (Transform target in VisibleTargets)
            {
                float targetDistance = Vector3.Distance(transform.position, target.position);

                if (targetDistance < distance) { distance = targetDistance; closestTarget = target; }
            }

            return closestTarget;
        }

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            VisibleTargets = new List<Transform>();
        }

        private void Update()
        {
            FindVisibleTargets();
        }

        void FindVisibleTargets()
        {
            VisibleTargets.Clear();

            Collider[] closeTargets = Physics.OverlapSphere(transform.position, minViewRadius, -1);

            for (int i = 0; i < closeTargets.Length; i++)
            {
                Transform target = closeTargets[i].transform;

                if (!target.CompareTag(targetTag)) continue;
                VisibleTargets.Add(target);
            }

            Collider[] targets = Physics.OverlapSphere(eyes.position, viewRadius, -1);

            for (int i = 0; i < targets.Length; i++)
            {
                Transform target = targets[i].transform;

                if (!target.CompareTag(targetTag)) continue;

                Vector3 dirToTarget = (target.position - eyes.position).normalized;

                if (Vector3.Angle(eyes.forward, dirToTarget) < viewAngle / 2)
                {
                    float disToTarget = Vector3.Distance(eyes.position, target.position);

                    if (Physics.Raycast(eyes.position, dirToTarget, out RaycastHit hit, disToTarget))
                    {
                        if (hit.transform == target) VisibleTargets.Add(target);
                    }
                }
            }
        }

        public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += eyes.eulerAngles.y;
            }

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}