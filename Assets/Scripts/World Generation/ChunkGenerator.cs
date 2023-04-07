using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    public class ChunkGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject terrainObject;

        public const float renderDistance = 128f;

        [SerializeField] private Transform player;

        public static int seed = 128;

        public static Vector2 playerPosition;
        private Vector2 previousPlayerPosition;

        private int chunkSize, chunksVisible;

        public static Vector2Int currentChunkCoord;

        Dictionary<Vector2, Chunk> chunkDictionary = new Dictionary<Vector2, Chunk>();
        List<Chunk> previousChunks = new List<Chunk>();


        public static WorldManager worldManager;


        private void Awake() => Init();

        private void Init()
        {
            worldManager = FindObjectOfType<WorldManager>();

            //since world gen is the first step, we're gonna init our seed.
            Random.InitState(seed); Noise.InitState(seed);

            chunkSize = TerrainGenerator.GetTerrainSize();
            chunksVisible = Mathf.RoundToInt(renderDistance / chunkSize);

            playerPosition = new Vector2(player.position.x, player.position.z);

            UpdateVisibleChunks();
        }

        private void Update()
        {
            UpdatePlayerPosition();
        }

        private void UpdatePlayerPosition()
        {
            playerPosition = new Vector2(player.position.x, player.position.z);

            if (playerPosition != previousPlayerPosition)
            {
                previousPlayerPosition = playerPosition;

                UpdateVisibleChunks();
            }
        }

        private void UpdateVisibleChunks()
        {
            //For loop to update previous chunks
            for (int i = 0; i < previousChunks.Count; i++)
            {
                previousChunks[i].SetVisible(false);
            }

            previousChunks.Clear();

            currentChunkCoord = new Vector2Int(Mathf.RoundToInt(playerPosition.x / chunkSize), Mathf.RoundToInt(playerPosition.y / chunkSize));

            //For loop to find viewed chunk coords
            for (int y = -chunksVisible; y <= chunksVisible; y++)
            {
                for (int x = -chunksVisible; x <= chunksVisible; x++)
                {
                    Vector2Int viewedChunkCoord = new Vector2Int(currentChunkCoord.x + x, currentChunkCoord.y + y);

                    //if we've already visited this chunk
                    if (chunkDictionary.ContainsKey(viewedChunkCoord))
                    {
                        //check if it is still visible
                        chunkDictionary[viewedChunkCoord].CheckVisibility();

                        //add to the previous chunks if visible
                        if (chunkDictionary[viewedChunkCoord].IsVisible())
                        {
                            previousChunks.Add(chunkDictionary[viewedChunkCoord]);
                        }
                    }

                    //instantiate new terrain chunk at the proper coord/size and with this object as the parent
                    else chunkDictionary.Add(viewedChunkCoord, new Chunk(terrainObject, viewedChunkCoord, chunkSize, gameObject.transform));
                }
            }
        }
    }

    public class Chunk
    {
        GameObject terrain;

        Vector2 pos;

        Vector2Int coord;

        //Bounds bounds;

        public Chunk(GameObject terrainObject, Vector2Int coord, int size, Transform parent)
        {
            this.coord = coord;
            pos = coord * size; //bounds = new Bounds(pos, Vector2.one * size);
            Vector3 worldPos = new Vector3(pos.x, 0, pos.y);

            terrain = Object.Instantiate(terrainObject);

            terrain.transform.position = worldPos;
            terrain.transform.parent = parent;

            TerrainGenerator tg = terrain.GetComponent<TerrainGenerator>();

            if (tg)
            {
                tg.SetOffset(coord * 16);
                tg.GenerateTerrain();
            }

            BiomeGenerator bg = terrain.GetComponentInChildren<BiomeGenerator>();

            if (bg)
            {
                bg.SetOffset(coord * 64);
                bg.GenerateBiome();
            }

            SetVisible(false);

            //ChunkGenerator.worldManager.RequestTerrainData(OnTerrainDataRecieved);
        }

        void OnTerrainDataRecieved(TerrainData terrainData)
        {
            TerrainGenerator tg = terrain.GetComponent<TerrainGenerator>();

            if (tg)
            {
                tg.SetOffset(coord * 16);
                tg.GenerateTerrain(terrainData);
            }

            BiomeGenerator bg = terrain.GetComponentInChildren<BiomeGenerator>();

            if (bg)
            {
                bg.SetOffset(coord * 64);
                bg.GenerateBiome();
            }

            //add in structure generator
        }

        public void CheckVisibility()
        {
            //float distance = Mathf.Sqrt(bounds.SqrDistance(ChunkGenerator.playerPosition));
            float distance = Vector2.Distance(pos, ChunkGenerator.playerPosition);
            bool visible = distance <= ChunkGenerator.renderDistance;
            SetVisible(visible);
        }

        public void SetVisible(bool visible) => terrain.SetActive(visible);

        public bool IsVisible() => terrain.activeSelf;
    }
}