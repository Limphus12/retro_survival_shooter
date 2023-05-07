using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace com.limphus.utilities
{
    public static class AINavigation
    {
        //sourced from - https://forum.unity.com/threads/solved-random-wander-ai-using-navmesh.327950/
        public static Vector3 RandomNavSphere(Vector3 origin, float dist, LayerMask layerMask)
        {
            Vector3 randDirection = Random.insideUnitSphere * dist + origin;

            NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layerMask);

            return navHit.position;
        }

        /// <summary>
        /// A function for converting world to navmesh position.
        /// </summary>
        /// <param name="worldPos">The world position to convert to a navmesh position.</param>
        /// <param name="layerMask">The layermask of the navmesh.</param>
        /// <returns></returns>
        public static Vector3 WorldToNavPoint(Vector3 worldPos, LayerMask layerMask)
        {
            NavMesh.SamplePosition(worldPos, out NavMeshHit navHit, Mathf.Infinity, layerMask);

            return navHit.position;
        }
    }
}