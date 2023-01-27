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

        [Space]
        [SerializeField] private Vector2Int currentChunk = Vector2Int.zero;

        [Header("References")]
        [SerializeField] private TerrainGenerator terrainGenerator;

        [SerializeField] private BiomeGenerator biomeGenerator;

        void Awake() => InitializeWorld();

        public void InitializeWorld()
        {
            //since world gen is the first step, we're gonna init our seed.
            //Random.InitState(seed);

            //generate our world!
            GenerateWorld();
        }

        public void GenerateWorld()
        {
            //we're gonna need to eventually grab the current chunk from our save

            //grab the terrain generator from our child and tell it to generate our terrain!
            //TerrainGenerator terrainGenerator = GetComponentInChildren<TerrainGenerator>();
            //if (terrainGenerator) terrainGenerator.GenerateTerrain(seed, currentChunk * 128);
            if (terrainGenerator) terrainGenerator.GenerateTerrain();

            //now we grab the biome generator from our child and tell it to place assets!
            //BiomeGenerator biomeGenerator = GetComponentInChildren<BiomeGenerator>();
            if (biomeGenerator) biomeGenerator.GenerateRuntimeBiome();

            Debug.Log("Generating World");
        }

        public void ClearWorld()
        {
            //TerrainGenerator terrainGenerator = GetComponentInChildren<TerrainGenerator>();
            if (terrainGenerator) terrainGenerator.ClearTerrain();

#if UNITY_EDITOR

            //BiomeGenerator biomeGenerator = GetComponentInChildren<BiomeGenerator>();
            if (biomeGenerator) biomeGenerator.EditorClearBiome();
#endif

            if (biomeGenerator) biomeGenerator.ClearBiome();

            Debug.Log("Clearing World");
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
            if (terrainGenerator) terrainGenerator.GenerateTerrain(seed, currentChunk * 128);

            //tell the biome generator to place assets!
            if (biomeGenerator) biomeGenerator.GenerateRuntimeBiome();
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

            if (GUILayout.Button("Generate")) worldGen.GenerateWorld();
            if (GUILayout.Button("Clear")) worldGen.ClearWorld();

            GUILayout.EndHorizontal();
        }
    }
#endif

    [System.Serializable]
    public struct WorldDataStruct
    {
        public MeshData meshData;
    }
}