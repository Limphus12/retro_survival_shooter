using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class ChunkGenerator : MonoBehaviour
    {
        public const float renderDistance = 160f;

        [SerializeField] private Transform player;

        public static Vector2 playerPosition;

        private int chunkSize, chunksVisible;

        private Vector2Int currentChunkCoord;

        Dictionary<Vector2, Chunk> chunkDictionary = new Dictionary<Vector2, Chunk>();
        List<Chunk> previousChunks = new List<Chunk>();


        private void Start()
        {
            chunkSize = TerrainGenerator.GetTerrainSize();

            chunksVisible = Mathf.RoundToInt(renderDistance / chunkSize);
        }

        private void Update()
        {
            UpdatePlayerPosition();
            UpdateVisibleChunks();
        }

        private void UpdatePlayerPosition()
        {
            playerPosition = new Vector2(player.position.x, player.position.z);
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
                    Vector2 viewedChunkCoord = new Vector2(currentChunkCoord.x + x, currentChunkCoord.y + y);

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
                    else chunkDictionary.Add(viewedChunkCoord, new Chunk(viewedChunkCoord, chunkSize, gameObject.transform));
                }
            }
        }
    }

    public class Chunk
    {
        GameObject terrainObject;

        Vector2 pos; Bounds bounds;

        public Chunk(Vector2 coord, int size, Transform parent)
        {
            pos = coord * size; bounds = new Bounds(pos, Vector2.one * size);

            Vector3 worldPos = new Vector3(pos.x, 0, pos.y);

            terrainObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            terrainObject.transform.position = worldPos;
            terrainObject.transform.localScale = Vector3.one * size / 10f;
            terrainObject.transform.parent = parent;

            SetVisible(false);
        }

        public void CheckVisibility()
        {
            float playerDistance = Mathf.Sqrt(bounds.SqrDistance(ChunkGenerator.playerPosition));
            bool visible = playerDistance <= ChunkGenerator.renderDistance;
            SetVisible(visible);
        }

        public void SetVisible(bool visible) => terrainObject.SetActive(visible);

        public bool IsVisible() => terrainObject.activeSelf;
    }
}