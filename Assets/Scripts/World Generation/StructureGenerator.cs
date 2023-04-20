using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    [System.Serializable]
    public struct StructureDataStruct
    {
        public StructureStruct primaryStructures, secondaryStructures, tertiaryStructures;
    }

    [System.Serializable]
    public struct StructureStruct
    {
        public StructureData structures;

        [Space]
        public float placementChance;
        public float heightPlacementOffset;

        [Space]
        public int gridSize;
        public int gridMultiplier, gridOffset;
    }

    public class StructureGenerator : MonoBehaviour
    {
        [Header("Attributes")]
        [SerializeField] private float raycastHeight;
        [SerializeField] private LayerMask layerMask;

        [Space]
        [SerializeField] private int gridSize;
        [SerializeField] private int gridMultiplier, gridOffset;

        [Space]
        [SerializeField] private Vector2Int offset;
        public void SetOffset(Vector2Int offset) => this.offset = offset;

        private StructureAreaStruct structureAreaStruct = new StructureAreaStruct{ structureAreas = new List<Vector3>(), structurePositions = new List<Vector3>() };

        public StructureDataStruct StructureData { private get; set; }

        [Space]
        [SerializeField] private TerrainGenerator terrainGenerator;

        //used for initial generation
        public void GenerateStructures()
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
            //need to do 3 asset loops for the primary, secondary and tertiary assets!
            if (StructureData.primaryStructures.structures != null) AssetLoop(StructureData.primaryStructures.gridSize, StructureData.primaryStructures.gridMultiplier, StructureData.primaryStructures.gridOffset, StructureData.primaryStructures.structures.assets, StructureData.primaryStructures.placementChance, StructureData.primaryStructures.heightPlacementOffset, true, false);

            if (StructureData.secondaryStructures.structures != null) AssetLoop(StructureData.secondaryStructures.gridSize, StructureData.secondaryStructures.gridMultiplier, StructureData.secondaryStructures.gridOffset, StructureData.secondaryStructures.structures.assets, StructureData.secondaryStructures.placementChance, StructureData.secondaryStructures.heightPlacementOffset, false, false);

            if (StructureData.tertiaryStructures.structures != null) AssetLoop(StructureData.tertiaryStructures.gridSize, StructureData.tertiaryStructures.gridMultiplier, StructureData.tertiaryStructures.gridOffset, StructureData.tertiaryStructures.structures.assets, StructureData.tertiaryStructures.placementChance, StructureData.tertiaryStructures.heightPlacementOffset, false, false);
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
            while (transform.childCount != 0)
            {
                DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);

                if (transform.childCount == 0) break;
            }

            structureAreaStruct.structurePositions.Clear();
            structureAreaStruct.structureAreas.Clear();
        }

        private void AssetLoop(int gridSize, int gridMultiplier, int gridOffset, GameObject[] assets, float assetPlacementChance, float placementOffset, bool rightAngles, bool flatLand)
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

                            Quaternion placementRotation;

                            if (rightAngles)
                            {
                                //calculate a 90 degree angle on the y axis
                                placementRotation = new Quaternion();

                                int i = Random.Range(0, 4);

                                if (i == 0) { placementRotation = Quaternion.Euler(0, 0, 0); }
                                else if (i == 1) { placementRotation = Quaternion.Euler(0, 90, 0); }
                                else if (i == 2) { placementRotation = Quaternion.Euler(0, 180, 0); }
                                else if (i == 3) { placementRotation = Quaternion.Euler(0, 270, 0); }
                            }

                            else
                            {
                                //calculate a random rotation on the y axis
                                placementRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                            }

                            //...placing down a random asset from the placeable asset array!
                            GameObject asset = Instantiate(assets[Random.Range(0, assets.Length)], placementPoint, placementRotation, gameObject.transform);

                            if (flatLand)
                            {
                                //grabs the asset remover script and chucks the extents in the structureAreas list
                                AssetRemover[] ar = asset.GetComponentsInChildren<AssetRemover>();

                                if (ar.Length > 0)
                                {
                                    foreach (AssetRemover assetRemover in ar)
                                    {
                                        if (assetRemover.AssetTag == "StructAsset")
                                        {
                                            structureAreaStruct.structurePositions.Add(assetRemover.transform.position);
                                            structureAreaStruct.structureAreas.Add(assetRemover.GetBoxExtents());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //if we hav the terrain generator, we can modify teh vertices via this method
            if (terrainGenerator && flatLand) terrainGenerator.ModifyVertices(structureAreaStruct);
        }
    }

    [System.Serializable]
    public struct StructureAreaStruct
    {
        public List<Vector3> structurePositions;
        public List<Vector3> structureAreas;
    }
}