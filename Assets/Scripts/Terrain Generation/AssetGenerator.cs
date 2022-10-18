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
        [SerializeField] private LayerMask mask;

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

            //if we have no assets, then skip!
            if (environmentalAssets.Length == 0) return;

            int j = 0, k = 0;

            //using a nested for loop - including our offset where we start at y & x = offset, and y & x increments until its less than or equal to gridsize - (grid offset * 2).
            for (int y = envGridOffset; y <= envGridSize.y - (envGridOffset * 2); y++)
            {
                for (int x = envGridOffset; x <= envGridSize.x - (envGridOffset * 2); x++)
                {
                    //generate a no. between 0 and 1
                    float z = Random.Range(0.0f, 1.0f);
                    
                    //if its greater than the placement chance
                    if (z <= envPlacementChance)
                    {
                        //calculate the current raycast position
                        Vector3 raycastPos = new Vector3(x * envGridMultiplier, raycastHeight, y * envGridMultiplier);

                        //and raycast downwards to hit the surface...
                        RaycastHit hit;
                        if (Physics.Raycast(raycastPos, Vector3.down, out hit, Mathf.Infinity, mask))
                        {
                            //calculate a little offset here
                            Vector3 placementPoint = new Vector3(hit.point.x, hit.point.y + envPlacementOffset, hit.point.z);

                            //...placing down a random asset from the placeable asset array!
                            Instantiate(environmentalAssets[Random.Range(0, environmentalAssets.Length - 1)], placementPoint, Quaternion.identity, gameObject.transform);

                            //increment i (for debug log)
                            j++;
                        }
                    }
                }
            }

            Debug.Log("Spawned " + j + " Environmental Assets");

            //if we have no assets, then skip!
            if (nonEnvironmentalAssets.Length == 0) return;

            //using a nested for loop - including our offset where we start at y & x = offset, and y & x increments until its less than or equal to gridsize - (grid offset * 2).
            for (int y = nonEnvGridOffset; y <= nonEnvGridSize.y - (nonEnvGridOffset * 2); y++)
            {
                for (int x = nonEnvGridOffset; x <= nonEnvGridSize.x - (nonEnvGridOffset * 2); x++)
                {
                    //generate a no. between 0 and 1
                    float z = Random.Range(0.0f, 1.0f);

                    //if its greater than the placement chance
                    if (z <= nonEnvPlacementChance)
                    {
                        //calculate the current raycast position
                        Vector3 raycastPos = new Vector3(x * nonEnvGridMultiplier, raycastHeight, y * nonEnvGridMultiplier);

                        //and raycast downwards to hit the surface...
                        RaycastHit hit;
                        if (Physics.Raycast(raycastPos, Vector3.down, out hit, Mathf.Infinity, mask))
                        {
                            //calculate a little offset here
                            Vector3 placementPoint = new Vector3(hit.point.x, hit.point.y + nonEnvPlacementOffset, hit.point.z);

                            //...placing down a random asset from the placeable asset array!
                            Instantiate(nonEnvironmentalAssets[Random.Range(0, nonEnvironmentalAssets.Length - 1)], placementPoint, Quaternion.identity, gameObject.transform);

                            //increment j (for debug log)
                            k++;
                        }
                    }
                }
            }

            Debug.Log("Spawned " + k + " Non-Environmental Assets");

        }
    }
}