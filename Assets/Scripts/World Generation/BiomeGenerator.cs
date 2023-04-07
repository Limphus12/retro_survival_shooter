using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class BiomeGenerator : MonoBehaviour
    {
        [Header("Attributes - Biome Data")]
        [SerializeField] private BiomeData biomeData;

        [Space]
        [SerializeField] private float raycastHeight;
        [SerializeField] private LayerMask layerMask;

        [Space]
        [SerializeField] private float placementChance = 0.5f;
        [SerializeField] private float heightPlacementOffset = -0.1f;

        [Space]
        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private int gridMultiplier, gridOffset;

        [Space]
        [SerializeField] private Vector2Int offset;

        int i = 0;

        public void SetOffset(Vector2Int offset) => this.offset = offset;

        //used for initial generation
        public void GenerateBiome()
        {
            //when generating our biome, always get rid of any previous assets
            ClearBiome();

            //then generate the new assets
            GenerateAssets();
        }

        //for use with the save system
        public void GenerateBiome(BiomeData biomeData)
        {
            //when generating our biome, always get rid of any previous assets
            ClearBiome();

            //then generate the new assets
            GenerateAssets(biomeData);
        }

        public void ClearBiome() => ClearAssets();
        public void EditorClearBiome() => EditorClearAssets();

        private void GenerateAssets()
        {
            if (!biomeData)
            {
                Debug.Log("We have no Biome assigned!");
                return;
            }

            AssetLoop(gridSize, gridMultiplier, gridOffset, biomeData.assets, placementChance, heightPlacementOffset);
        }

        //for use with the save system
        private void GenerateAssets(BiomeData biomeData)
        {
            if (!biomeData)
            {
                Debug.Log("We have no Biome assigned!");
                return;
            }

            AssetLoop(gridSize, gridMultiplier, gridOffset, biomeData.assets, placementChance, heightPlacementOffset);
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

        private void AssetLoop(Vector2Int gridSize, int gridMultiplier, int gridOffset, GameObject[] assets, float assetPlacementChance, float placementOffset)
        {
            i = 0;

            //if we have no assets, then skip!
            if (assets.Length == 0) return;

            //using a nested for loop - including our offset where we start at y & x = offset, and y & x increments until its less than or equal to gridsize - grid offset.
            for (int y = gridOffset; y <= gridSize.y - gridOffset; y++)
            {
                for (int x = gridOffset; x <= gridSize.x - gridOffset; x++)
                {
                    //generate a no. between 0 and 1
                    float z = Random.Range(0.0f, 1.0f);

                    //if its greater than the placement chance
                    if (z <= assetPlacementChance)
                    {
                        //generate a random offset
                        Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f) + offset.x, placementOffset, Random.Range(-1f, 1f) + offset.y);

                        //calculate the current raycast position, adding in our random offset
                        Vector3 raycastPos = new Vector3((x * gridMultiplier) + offset.x + randomOffset.x, raycastHeight, (y * gridMultiplier) + offset.y + randomOffset.y);

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

                            i++; //increment i
                        }
                    }
                }
            }
        }
    }
}