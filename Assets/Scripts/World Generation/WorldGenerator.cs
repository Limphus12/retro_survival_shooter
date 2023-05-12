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
        public StructureDataStruct structureData;
    }

    public class WorldGenerator : MonoBehaviour
    {
        public static int Seed;

        [Space]
        [SerializeField] private Vector2Int currentChunk = Vector2Int.zero;

        [Space]
        [SerializeField] private WorldDataStruct[] worldData;

        [Header("References")]
        [SerializeField] private TerrainGenerator terrainGenerator;

        [SerializeField] private BiomeGenerator biomeGenerator;

        [SerializeField] private StructureGenerator structureGenerator;

        [SerializeField] private NavMeshGenerator navMeshGenerator;

        [SerializeField] private AIGenerator aiGenerator;

        public WorldDataStruct CurrentWorldData { get; private set; }

        public static void SetCurrentSeed(int i) => Seed = i;

        void Awake() => InitializeWorld();

        public void InitializeWorld()
        {
            Random.InitState(Seed); Noise.InitState(Seed); //since world gen is the first step, we're gonna init our seed.

            //generate our world!
            GenerateWorld();
        }

        public void EditorInitializeWorld()
        {
            Random.InitState(Seed); Noise.InitState(Seed); //since world gen is the first step, we're gonna init our seed.

            //generate our world!
            EditorGenerateWorld();
        }

        public void GenerateWorld()
        {
            //firstly clear the world of previous terrain, biomes etc.
            ClearWorld();

            //int i = Random.Range(0, worldData.Length) + currentChunk.x + currentChunk.y;

            int j = 0;

            //doing mathf.abs cos chunks can go into the negative
            for (int i = 0; i < Mathf.Abs(currentChunk.x + currentChunk.y); i++)
            {
                j++;

                //not gonna reset j to 0, instead will pick a random (in range) value
                if (j > worldData.Length - 1) j = Random.Range(0, worldData.Length);
            }

            CurrentWorldData = worldData[j];

            //Debug.Log(worldData[j].name);

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

            if (structureGenerator)
            {
                structureGenerator.StructureData = CurrentWorldData.structureData;
                structureGenerator.SetOffset(currentChunk);
                structureGenerator.GenerateStructures();
            }

            if (navMeshGenerator) navMeshGenerator.GenerateNavMesh();

            if (aiGenerator)
            {
                aiGenerator.SetOffset(currentChunk);
                aiGenerator.GenerateAI();
            }
        }

        public void EditorGenerateWorld()
        {
            //firstly clear the world of previous terrain, biomes etc.
            EditorClearWorld();

            //int i = Random.Range(0, worldData.Length) + currentChunk.x + currentChunk.y;

            int j = 0;

            for (int i = 0; i < Mathf.Abs(currentChunk.x + currentChunk.y); i++)
            {
                j++;

                //not gonna reset j to 0, instead will pick a random (in range) value
                if (j > worldData.Length - 1) j = Random.Range(0, worldData.Length);
            }

            CurrentWorldData = worldData[j];

            //Debug.Log(worldData[j].name);

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

            if (structureGenerator)
            {
                structureGenerator.StructureData = CurrentWorldData.structureData;
                structureGenerator.SetOffset(currentChunk);
                structureGenerator.GenerateStructures();
            }

            if (navMeshGenerator) navMeshGenerator.GenerateNavMesh();

            if (aiGenerator)
            {
                aiGenerator.SetOffset(currentChunk);
                aiGenerator.GenerateAI();
            }
        }

        public void ClearWorld()
        {
            if (terrainGenerator) terrainGenerator.ClearTerrain();

            if (biomeGenerator) biomeGenerator.ClearBiome();

            if (structureGenerator) structureGenerator.ClearStructures();
        }

        public void EditorClearWorld()
        {
            if (terrainGenerator) terrainGenerator.ClearTerrain();

            if (biomeGenerator) biomeGenerator.EditorClearBiome();

            if (structureGenerator) structureGenerator.EditorClearStructures();
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

            if (GUILayout.Button("Generate")) worldGen.EditorInitializeWorld();
            if (GUILayout.Button("Clear")) worldGen.EditorClearWorld();

            GUILayout.EndHorizontal();
        }
    }
#endif
}