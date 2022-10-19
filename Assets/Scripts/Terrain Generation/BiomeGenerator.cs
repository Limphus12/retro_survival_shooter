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
        [SerializeField] private float envHeightPlacementOffset = 2.0f, envPlacementOffset = -0.1f;

        [Space]
        [SerializeField] private float nonEnvPlacementChance = 0.05f;
        [SerializeField] private float nonEnvHeightPlacementOffset = 1.0f, nonEnvPlacementOffset = -0.1f;

        [Header("Grid Variables")]
        [SerializeField] private Vector2Int envGridSize;
        [SerializeField] private int envGridMultiplier, envGridOffset;

        [Space]
        [SerializeField] private Vector2Int nonEnvGridSize;
        [SerializeField] private int nonEnvGridMultiplier, nonEnvGridOffset;
        
        private void SetSeed(int seed)
        {
            //init the rng with our seed
            Random.InitState(seed);
        }

        public void GenerateAssets(int seed)
        {
            //if we have children
            if (transform.childCount != 0)
            {
                //delete all of our children
                for (int i = 0; i < transform.childCount; i++)
                {
                    Destroy(transform.GetChild(i));
                }
            }

            SetSeed(seed);

            Loop(envGridSize, envGridMultiplier, envGridOffset, environmentalAssets, envPlacementChance, envHeightPlacementOffset);
            Debug.Log("Spawned " + i + " Envionmental Assets");

            Loop(nonEnvGridSize, nonEnvGridMultiplier, nonEnvGridOffset, nonEnvironmentalAssets, nonEnvPlacementChance, nonEnvHeightPlacementOffset);
            Debug.Log("Spawned " + i + " Non-Envionmental Assets");
        }

        int i = 0;

        private void Loop(Vector2Int gridSize, int gridMultiplier, int gridOffset, GameObject[] assets, float assetPlacementChance, float placementOffset)
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

                            //increment k (for debug log)
                            i++;
                        }
                    }
                }
            }
        }
    }
}