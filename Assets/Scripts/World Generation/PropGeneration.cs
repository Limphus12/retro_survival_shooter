using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class PropGeneration : MonoBehaviour
    {
        [Header("Attributes - Props")]
        [SerializeField] private PropGenerationPoint[] propGenerationPoints;

        private void Start()
        {
            if (propGenerationPoints.Length <= 0) return; //if we have no prop points, then skip!

            for (int i = 0; i < propGenerationPoints.Length; i++)
            {
                //call the generate function on each of these points!
                propGenerationPoints[i].Generate();
            }
        }
    }
}
