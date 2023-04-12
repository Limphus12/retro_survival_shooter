using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.limphus.retro_survival_shooter
{
    public class WorldGenerator : MonoBehaviour
    {
        [SerializeField] private int seed;

        [Space]
        [SerializeField] private Vector2Int currentChunk = Vector2Int.zero;

        [Header("References")]
        [SerializeField] private TerrainGenerator terrainGenerator;

        [SerializeField] private BiomeGenerator biomeGenerator;

        [SerializeField] private StructureGenerator structureGenerator;

        void Awake() => InitializeWorld();

        public void InitializeWorld()
        {
            //since world gen is the first step, we're gonna init our seed.
            Random.InitState(seed); Noise.InitState(seed);

            //generate our world!
            GenerateWorld();
        }

        public void GenerateWorld()
        {
            //we're gonna need to eventually grab the current chunk from our save

            //grab the terrain generator from our child and tell it to generate our terrain!
            //TerrainGenerator terrainGenerator = GetComponentInChildren<TerrainGenerator>();
            if (terrainGenerator) terrainGenerator.SetSeed(seed);

            if (terrainGenerator) terrainGenerator.GenerateTerrain(currentChunk);
            //if (terrainGenerator) terrainGenerator.GenerateTerrain(seed);

            //now we grab the biome generator from our child and tell it to place assets!
            //BiomeGenerator biomeGenerator = GetComponentInChildren<BiomeGenerator>();
            if (biomeGenerator) biomeGenerator.GenerateBiome();

            if (structureGenerator) structureGenerator.GenerateStructures();

            Debug.Log("Generating World");
        }

        public void ClearWorld()
        {
            Debug.Log("Clearing World");

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

            if (b) return;
#endif

            if (biomeGenerator) biomeGenerator.ClearBiome();

            if (structureGenerator) structureGenerator.ClearStructures();
        }

        //we're prolly gonna call this from the borders around our world.
        public void MoveChunk(int i)
        {
            Debug.Log("Move Chunk");

            //north
            if (i <= 0) currentChunk.y++;

            //east
            else if (i == 1) currentChunk.x++;

            //south
            else if (i == 2) currentChunk.y--;

            //west
            else if (i >= 3) currentChunk.x--;

            //tell the terrain generator to generate our terrain!
            if (terrainGenerator) terrainGenerator.GenerateTerrain();

            //tell the biome generator to place assets!
            if (biomeGenerator) biomeGenerator.GenerateBiome();

            //tell the structure generator to place assets!
            if (structureGenerator) structureGenerator.GenerateStructures();
        }

        public Vector2Int GetCurrentChunk()
        {
            return currentChunk;
        }

        public void SetCurrentChunk(Vector2Int chunk)
        {
            currentChunk = chunk;

            //generate our world!
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

    [System.Serializable]
    public struct WorldDataStruct
    {
        public TerrainData meshData;
    }
}