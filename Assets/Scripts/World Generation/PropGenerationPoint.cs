using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace com.limphus.retro_survival_shooter
{
    public class PropGenerationPoint : MonoBehaviour
    {
        [Header("Attributes - Prop Data")]
        [SerializeField] private PropData propData;

        [Space]
        [SerializeField] private Transform[] propSpawnPoints;

        [Space]
        [SerializeField] private float placementChance = 0.5f;

        int i = 0;

        public void Generate()
        {
            AssetLoop(propData.assets, placementChance);
            Debug.Log("Spawned " + i + " Prop Assets");
        }

        private void AssetLoop(GameObject[] assets, float assetPlacementChance)
        {
            if (assets.Length == 0)
            {
                Debug.Log("No Assets to Place!");
                return; //if we have no assets, then skip!
            }

            i = 0;

            //using a loop to go through each of our prop spawn points
            for (int j = 0; j < propSpawnPoints.Length; j++)
            {
                //generate a no. between 0 and 1
                float z = Random.Range(0.0f, 1.0f);

                //if its greater than the placement chance
                if (z <= assetPlacementChance)
                {
                    //place down a random asset from the placeable asset array, and set that prop spawn point as the parent
                    Instantiate(assets[Random.Range(0, assets.Length)], propSpawnPoints[j].position, propSpawnPoints[j].rotation, propSpawnPoints[j]);

                    i++; //increment i (for debug log)
                }
            }
        }
    }
}