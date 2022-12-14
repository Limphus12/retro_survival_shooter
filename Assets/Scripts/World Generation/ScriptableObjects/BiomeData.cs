using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(fileName = "BiomeData", menuName = "World Generation/Biome")]
    public class BiomeData : ScriptableObject
    {
        [Header("Assets")]
        public GameObject[] environmentalAssets;

        [Space]
        public GameObject[] nonEnvironmentalAssets;
    }
}