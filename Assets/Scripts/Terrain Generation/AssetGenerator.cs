using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class AssetGenerator : MonoBehaviour
    {
        [Header("Asset Variables")]
        [SerializeField] private GameObject[] environmentalAssets, nonEnvironmentalAssets;

        [Header("Placement Variables")]
        [SerializeField] private float raycastHeight;
        [SerializeField] private LayerMask layerMask;

        [Space]
        [SerializeField] private float envPlacementChance = 0.5f;
        [SerializeField] private float envPlacementOffset = -0.1f;

        [Space]
        [SerializeField] private float nonEnvPlacementChance = 0.05f;
        [SerializeField] private float nonEnvPlacementOffset = -0.1f;

        [Header("Grid Variables")]
        [SerializeField] private Vector2Int envGridSize;
        [SerializeField] private int envGridMultiplier, envGridOffset;

        [Space]
        [SerializeField] private Vector2Int nonEnvGridSize;
        [SerializeField] private int nonEnvGridMultiplier, nonEnvGridOffset;

        public void GenerateAssets()
        {
            //if we have clildren
            if (transform.childCount != 0)
            {
                //delete all of our children
                for (int i = 0; i < transform.childCount; i++)
                {
                    Destroy(transform.GetChild(i));
                }
            }

            Loop(envGridSize, envGridMultiplier, envGridOffset, environmentalAssets, envPlacementChance);
            Loop(nonEnvGridSize, nonEnvGridMultiplier, nonEnvGridOffset, nonEnvironmentalAssets, nonEnvPlacementChance);
        }

        private void Loop(Vector2Int gridSize, int gridMultiplier, int gridOffset, GameObject[] assets, float assetPlacementChance)
        {
            int i = 0;

            //if we have no assets, then skip!
            if (assets.Length == 0) return;

            //using a nested for loop - including our offset where we start at y & x = offset, and y & x increments until its less than or equal to gridsize - (grid offset * 2).
            for (int y = gridOffset; y <= gridSize.y - (gridOffset * 2); y++)
            {
                for (int x = gridOffset; x <= gridSize.x - (gridOffset * 2); x++)
                {
                    //generate a no. between 0 and 1
                    float z = Random.Range(0.0f, 1.0f);

                    //if its greater than the placement chance
                    if (z <= assetPlacementChance)
                    {
                        //calculate the current raycast position
                        Vector3 raycastPos = new Vector3(x * gridMultiplier, raycastHeight, y * gridMultiplier);

                        //and raycast downwards to hit the surface...
                        RaycastHit hit;
                        if (Physics.Raycast(raycastPos, Vector3.down, out hit, Mathf.Infinity, layerMask))
                        {
                            //generate a random offset
                            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), nonEnvPlacementOffset, Random.Range(-10f, 10f));

                            //calculate the position here
                            Vector3 placementPoint = new Vector3(hit.point.x + randomOffset.x, hit.point.y + randomOffset.y, hit.point.z + randomOffset.z);

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

            Debug.Log("Spawned " + i + " Non-Environmental Assets");
        }
    }
}