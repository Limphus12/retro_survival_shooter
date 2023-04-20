using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.limphus.retro_survival_shooter
{
    [System.Serializable]
    public struct WorldDataStruct
    {
        public string name;

        [Space]
        public TerrainDataStruct terrainData;
        public BiomeDataStruct biomeData;
    }

    public class WorldGenerator : MonoBehaviour
    {
        [SerializeField] private int seed;
        public static int Seed;

        [Space]
        [SerializeField] private Vector2Int currentChunk = Vector2Int.zero;

        [Space]
        [SerializeField] private WorldDataStruct[] worldData;

        [Header("References")]
        [SerializeField] private TerrainGenerator terrainGenerator;

        [SerializeField] private BiomeGenerator biomeGenerator;

        [SerializeField] private StructureGenerator structureGenerator;

        public WorldDataStruct CurrentWorldData { get; private set; }

        void Awake() => InitializeWorld();

        public void InitializeWorld()
        {
            Random.InitState(seed); Noise.InitState(seed); //since world gen is the first step, we're gonna init our seed.

            //generate our world!
            GenerateWorld();
        }

        public void GenerateWorld()
        {
            //firstly clear the world of previous terrain, biomes etc.
            ClearWorld();

            //int i = Random.Range(0, worldData.Length) + currentChunk.x + currentChunk.y;

            int j = 0;

            for (int i = 0; i < currentChunk.x + currentChunk.y; i++)
            {
                j++;

                //not gonna reset j to 0, instead will pick a random (in range) value
                if (j > worldData.Length - 1) j = Random.Range(0, worldData.Length);
            }

            CurrentWorldData = worldData[j];

            if (terrainGenerator)
            {
                terrainGenerator.TerrainData = CurrentWorldData.terrainData;
                terrainGenerator.SetOffset(currentChunk);
                terrainGenerator.GenerateTerrain();
            }

            if (biomeGenerator)
            {
                biomeGenerator.BiomeData = CurrentWorldData.biomeData;
                biomeGenerator.SetOffset(currentChunk);
                biomeGenerator.GenerateBiome();
            }

            //if (structureGenerator) structureGenerator.GenerateStructures();
        }

        public void ClearWorld()
        {
            //TerrainGenerator terrainGenerator = GetComponentInChildren<TerrainGenerator>();
            if (terrainGenerator) terrainGenerator.ClearTerrain();

#if UNITY_EDITOR

            //Non-Runtime Functions are Called Here

            bool b = false;

            if (biomeGenerator)
            {
                biomeGenerator.EditorClearBiome();

                b = true;
            }

            if (structureGenerator)
            {
                structureGenerator.EditorClearStructures();

                b = true;
            }
#endif

            if (b) return;

            if (biomeGenerator) biomeGenerator.ClearBiome();

            if (structureGenerator) structureGenerator.ClearStructures();
        }

        public void Travel(CardinalDirection direction)
        {
            switch (direction)
            {
                case CardinalDirection.NORTH: currentChunk.y++;
                    break;

                case CardinalDirection.EAST: currentChunk.x++;
                    break;

                case CardinalDirection.SOUTH: currentChunk.y--;
                    break;

                case CardinalDirection.WEST: currentChunk.x++;
                    break;
            }

            GenerateWorld();
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

            if (GUILayout.Button("Generate")) worldGen.InitializeWorld();
            if (GUILayout.Button("Clear")) worldGen.ClearWorld();

            GUILayout.EndHorizontal();
        }
    }
#endif
}