using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class LootGenerator : MonoBehaviour
    {
        [Header("Attributes - Loot Data")]
        [SerializeField] private LootData lootData;

        [Space]
        [SerializeField] private Transform[] lootSpawnPoints;

        [Space]
        [SerializeField] private float placementChance = 0.5f;

        int i = 0;

        private void Start() => Generate();

        public void Generate()
        {
            AssetLoop(lootData.assets, placementChance);
            Debug.Log("Spawned " + i + " Loot Assets");
        }

        private void AssetLoop(GameObject[] assets, float assetPlacementChance)
        {
            if (assets.Length == 0)
            {
                Debug.Log("No Assets to Place!");
                return; //if we have no assets, then skip!
            }

            i = 0;

            //generate a no. between 0 and 1
            float z = Random.Range(0.0f, 1.0f);

            for (int j = 0; j < lootSpawnPoints.Length; j++)
            {
                //if its greater than the placement chance
                if (z <= assetPlacementChance)
                {
                    //place down a random asset from the placeable asset array, and set that loot spawn point as the parent
                    Instantiate(assets[Random.Range(0, assets.Length)], lootSpawnPoints[j].position, lootSpawnPoints[j].rotation, lootSpawnPoints[j]);

                    i++; //increment i (for debug log)
                }
            }
        }
    }
}