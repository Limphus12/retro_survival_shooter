using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(fileName = "TerrainData", menuName = "World Generation/Terrain")]
    public class TerrainData : ScriptableObject
    {
        [Header("Attributes")]
        public float noiseScale = 1.0f;
        public int octaves = 4;
        [Range(0f, 1f)] public float persistance = 0.5f;
        public float lacunarity = 2.0f, heightMultiplier = 2.0f;
    }
}