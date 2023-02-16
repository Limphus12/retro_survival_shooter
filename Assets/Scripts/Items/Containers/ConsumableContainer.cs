using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class ConsumableContainer : Container
    {
        [Header("Attributes - Consumable")]
        [SerializeField] private ConsumableContainerData data;

        private void Update() => CheckLoot();

        protected override void CheckLoot()
        {
            if (!isLooting)
            {

            }
        }

        protected override void StartLoot()
        {
            isLooting = true;

            Invoke(nameof(Loot), lootTime);
        }

        protected override void Loot()
        {
            //provide the player with a random loot fom the data var
            //later on, we'd need to roll for a rarity, but rn we'll just pick a random item
            int i = Random.Range(0, data.lootables.Length);

            //if we have the player inventory reference, add the item to their inventory.
            //if (playerInventory) playerInventory.AddInventoryItem(data.lootables[i]);

            EndLoot();
        }

        protected override void EndLoot()
        {
            isLooting = false;
        }
    }
}