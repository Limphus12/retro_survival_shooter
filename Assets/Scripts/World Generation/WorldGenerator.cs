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

        void Awake() => Generate();

        public void Generate()
        {
            //since world gen is the first step, we're gonna init our seed.
            Random.InitState(seed);

            //we're gonna need to eventually grab the current chunk from our save

            //grab the terrain generator from our child and tell it to generate our terrain!
            //TerrainGenerator terrainGenerator = GetComponentInChildren<TerrainGenerator>();
            if (terrainGenerator) terrainGenerator.GenerateTerrain(seed, currentChunk * 128);

            //now we grab the biome generator from our child and tell it to place assets!
            //BiomeGenerator biomeGenerator = GetComponentInChildren<BiomeGenerator>();
            if (biomeGenerator) biomeGenerator.GenerateBiome();

            Debug.Log("Generating World");
        }

        public void Clear()
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

            //was gonna use a vec2int for our paramater
            //but we cant use that in our event :/

            //update our current chunk
            //currentChunk += direction;

            //north
            if (i <= 0)
            {
                currentChunk.y++;
            }

            //east
            else if (i == 1)
            {
                currentChunk.x++;
            }

            //south
            else if (i == 2)
            {
                currentChunk.y--;
            }

            //west
            else if (i >= 3)
            {
                currentChunk.x--;
            }

            //MATHS: if our current chunk is (-10, 9)
            //then we'll end up with (-1280, 1152) on the offset
            //if we have a 0 at either chunk (x or y), then the
            //maths should just give us 0, so no offset.

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

            //now we reload our world!

            //tell the terrain generator to generate our terrain!
            if (terrainGenerator) terrainGenerator.GenerateTerrain(seed, currentChunk * 128);

            //tell the biome generator to place assets!
            if (biomeGenerator) biomeGenerator.GenerateRuntimeBiome();
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