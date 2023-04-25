using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    [System.Serializable]
    public struct BiomeDataStruct
    {
        [Space]
        public BiomeData biomeData;

        [Space]
        public float placementChance;
        public float heightPlacementOffset;
    }

    public class BiomeGenerator : MonoBehaviour
    {
        [Header("Attributes - Biome Size")]
        [Space]
        [SerializeField] private int gridSize;
        [SerializeField] private int gridMultiplier, gridOffset;

        [Space]
        [SerializeField] private float raycastHeight;
        [SerializeField] private LayerMask layerMask;

        [Space]
        [SerializeField] private Vector2Int offset;

        public BiomeDataStruct BiomeData { private get; set; }

        public void SetOffset(Vector2Int offset) => this.offset = offset;

        //used for initial generation
        public void GenerateBiome()
        {
            //when generating our biome, always get rid of any previous assets
            ClearBiome();

            //then generate the new assets
            GenerateAssets();
        }

        public void ClearBiome() => ClearAssets();
        public void EditorClearBiome() => EditorClearAssets();

        private void GenerateAssets()
        {
            if (BiomeData.Equals(null))
            {
                Debug.Log("We have no Biome assigned!");
                return;
            }

            if (BiomeData.biomeData.assets != null) AssetLoop(gridSize, gridMultiplier, gridOffset, BiomeData.biomeData.assets, BiomeData.placementChance, BiomeData.heightPlacementOffset);
        }

        private void ClearAssets()
        {
            //if we have children
            if (transform.childCount != 0)
            {
                //delete all of our children
                for (int i = 0; i < transform.childCount; i++)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
            }
        }

        private void EditorClearAssets()
        {
            while (transform.childCount != 0)
            {
                DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);

                if (transform.childCount == 0) break;
            }
        }

        private void AssetLoop(int gridSize, int gridMultiplier, int gridOffset, GameObject[] assets, float assetPlacementChance, float placementOffset)
        {
            //if we have no assets, then skip!
            if (assets.Length == 0) return;

            float[,] heightMap = Noise.SimpleNoiseMap(gridSize + 1 - gridOffset, gridSize + 1 - gridOffset, gridMultiplier, offset);

            //using a nested for loop - including our offset where we start at y & x = offset, and y & x increments until its less than or equal to gridsize - grid offset.
            for (int y = gridOffset; y <= gridSize - gridOffset; y++)
            {
                for (int x = gridOffset; x <= gridSize - gridOffset; x++)
                {
                    //generate a no. between 0 and 1
                    //float z = Random.Range(0.0f, 1.0f);

                    float z = Mathf.Abs(heightMap[x, y]);

                    //if its less than the placement chance
                    if (z <= assetPlacementChance)
                    {
                        //generate a random offset
                        Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), placementOffset, Random.Range(-1f, 1f));

                        //calculate the current raycast position, adding in our random offset
                        Vector3 raycastPos = new Vector3((x * gridMultiplier) + randomOffset.x, raycastHeight, (y * gridMultiplier) + randomOffset.z);

                        //and raycast downwards to hit the surface...
                        RaycastHit hit;
                        if (Physics.Raycast(raycastPos, Vector3.down, out hit, Mathf.Infinity, layerMask))
                        {
                            //calculate the position here
                            Vector3 placementPoint = new Vector3(hit.point.x, hit.point.y + randomOffset.y, hit.point.z);

                            //calculate a random rotation on the y axis
                            Quaternion placementRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

                            //...placing down a random asset from the placeable asset array!
                            Instantiate(assets[Random.Range(0, assets.Length)], placementPoint, placementRotation, gameObject.transform);
                        }
                    }
                }
            }
        }
    }
}