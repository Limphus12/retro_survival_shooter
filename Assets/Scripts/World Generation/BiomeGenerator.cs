using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class BiomeGenerator : MonoBehaviour
    {
        [Header("Asset Variables")]
        [SerializeField] private GameObject[] environmentalAssets, nonEnvironmentalAssets;

        [Header("Placement Variables")]
        [SerializeField] private float raycastHeight;
        [SerializeField] private LayerMask layerMask;

        [Space]
        [SerializeField] private float envPlacementChance = 0.5f;
        [SerializeField] private float envHeightPlacementOffset = 2.0f;

        [Space]
        [SerializeField] private float nonEnvPlacementChance = 0.05f;
        [SerializeField] private float nonEnvHeightPlacementOffset = 1.0f;

        [Header("Grid Variables")]
        [SerializeField] private Vector2Int envGridSize;
        [SerializeField] private int envGridMultiplier, envGridOffset;

        [Space]
        [SerializeField] private Vector2Int nonEnvGridSize;
        [SerializeField] private int nonEnvGridMultiplier, nonEnvGridOffset;

        int i = 0;

        public void GenerateBiome()
        {
            //init the rng with our seed
            //EDIT - Moved to WorldGenerator
            //Random.InitState(seed);

#if UNITY_EDITOR
            //just calls the remove assets function
            EditorClearBiome();
#endif

            //when generating our biome, always get rid of any previous assets
            RemoveAssets();

            //then generate the new assets
            GenerateAssets();
        }

        public void GenerateRuntimeBiome()
        {
            //when generating our biome, always get rid of any previous assets
            ClearBiome();

            //then generate the new assets
            GenerateAssets();
        }

        public void ClearBiome()
        {
            //just calls the remove assets function
            RemoveAssets();
        }

        public void EditorClearBiome()
        {
            //just calls the remove assets function
            EditorRemoveAssets();
        }

        private void GenerateAssets()
        {
            AssetLoop(envGridSize, envGridMultiplier, envGridOffset, environmentalAssets, envPlacementChance, envHeightPlacementOffset);
            Debug.Log("Spawned " + i + " Envionmental Assets");

            AssetLoop(nonEnvGridSize, nonEnvGridMultiplier, nonEnvGridOffset, nonEnvironmentalAssets, nonEnvPlacementChance, nonEnvHeightPlacementOffset);
            Debug.Log("Spawned " + i + " Non-Envionmental Assets");
        }

        private void RemoveAssets()
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

        private void EditorRemoveAssets()
        {
            Debug.Log("Editor - Removing Assets");

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
                            Instantiate(assets[Random.Range(0, assets.Length - 1)], placementPoint, placementRotation, gameObject.transform);

                            //increment i (for debug log)
                            i++;
                        }
                    }
                }
            }
        }
    }
}