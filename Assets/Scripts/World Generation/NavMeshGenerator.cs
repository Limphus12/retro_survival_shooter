using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace com.limphus.retro_survival_shooter
{
    public class NavMeshGenerator : MonoBehaviour
    {
        [SerializeField] private NavMeshSurface navMeshSurface;

        public void GenerateNavMesh()
        {
            //call the build nav mesh
            navMeshSurface.BuildNavMesh();
        }
    }
}