using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.limphus.retro_survival_shooter
{
    public class WorldGenerator : MonoBehaviour
    {
        [SerializeField] private int seed;

        void Awake() => Generate();

        public void Generate()
        {
            //grab the terrain generator from our child and tell it to generate our terrain!
            TerrainGenerator terrainGenerator = GetComponentInChildren<TerrainGenerator>();
            if (terrainGenerator) terrainGenerator.GenerateTerrain(seed);
        }

        public void Clear()
        {
            TerrainGenerator terrainGenerator = GetComponentInChildren<TerrainGenerator>();
            if (terrainGenerator) terrainGenerator.ClearTerrain();

            BiomeGenerator biomeGenerator = GetComponentInChildren<BiomeGenerator>();
            if (biomeGenerator) biomeGenerator.ClearBiome();

            Debug.Log("Clearing Biome");
        }
    }



    #if UNITY_EDITOR
    [CustomEditor(typeof(WorldGenerator))]
    public class WorldGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var worldGen = (WorldGenerator)target;
            if (worldGen == null) return;

            GUILayout.Space(16);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Generate")) worldGen.Generate();
            if (GUILayout.Button("Clear")) worldGen.Clear();

            GUILayout.EndHorizontal();
        }
    }
    #endif
}