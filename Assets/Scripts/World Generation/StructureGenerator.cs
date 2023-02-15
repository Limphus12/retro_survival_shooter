using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class StructureGenerator : MonoBehaviour
    {
        [Header("Attributes - Structure Data")]
        [SerializeField] private StructureData structureData;

        [Space]
        [SerializeField] private float raycastHeight;
        [SerializeField] private LayerMask layerMask;

        [Space]
        [SerializeField] private float placementChance = 0.5f;
        [SerializeField] private float heightPlacementOffset = -0.1f;

        [Space]
        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private int gridMultiplier, gridOffset;

        private StructureAreaStruct structureAreaStruct = new StructureAreaStruct{ structureAreas = new List<Vector3>(), structurePositions = new List<Vector3>() };

        [Space]
        [SerializeField] private TerrainGenerator terrainGenerator;

        int i = 0;

        //used for initial generation
        public void GenerateRuntimeStructures()
        {
            //when generating our structures, always get rid of any previous assets
            ClearStructures();

            //then generate the new assets
            GenerateAssets();
        }

        public void ClearStructures() => ClearAssets();
        public void EditorClearStructures() => EditorClearAssets();

        private void GenerateAssets()
        {
            if (!structureData)
            {
                Debug.Log("We have no Structures assigned!");
                return;
            }

            AssetLoop(gridSize, gridMultiplier, gridOffset, structureData.assets, placementChance, heightPlacementOffset);
            Debug.Log("Spawned " + i + " Structure Assets");
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

            structureAreaStruct.structurePositions.Clear();
            structureAreaStruct.structureAreas.Clear();
        }

        private void EditorClearAssets()
        {
            Debug.Log("Editor - Removing Assets");

            while (transform.childCount != 0)
            {
                DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);

                if (transform.childCount == 0) break;
            }

            structureAreaStruct.structurePositions.Clear();
            structureAreaStruct.structureAreas.Clear();
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
                            GameObject asset = Instantiate(assets[Random.Range(0, assets.Length)], placementPoint, placementRotation, gameObject.transform);
                            //GameObject asset = Instantiate(assets[Random.Range(0, assets.Length - 1)], placementPoint, Quaternion.identity, gameObject.transform);

                            //grabs the asset remover script and chucks the extents in the structureAreas list
                            AssetRemover ar = asset.GetComponentInChildren<AssetRemover>(); 
                            
                            if (ar)
                            {
                                structureAreaStruct.structurePositions.Add(asset.transform.position);
                                structureAreaStruct.structureAreas.Add(ar.GetBoxExtents());
                            }

                            //increment i (for debug log)
                            i++;
                        }
                    }
                }
            }

            //if we hav the terrain generator, we can modify teh vertices via this method
            if (terrainGenerator) terrainGenerator.ModifyVertices(structureAreaStruct);
        }
    }

    [System.Serializable]
    public struct StructureAreaStruct
    {
        public List<Vector3> structurePositions;
        public List<Vector3> structureAreas;
    }
}