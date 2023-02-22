using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class ConsumableContainer : Container
    {
        [Header("Attributes - Consumable")]
        [SerializeField] private ConsumableContainerData data;

        public override void StartInteract() => isInteracting = true;

        public override void StopInteract() => isInteracting = false;

        public override bool IsLooting() => isLooting;

        public override void StartLoot()
        {
            isLooting = true;

            Debug.Log("Started Looting the Consumable Containter!");

            Invoke(nameof(Loot), lootTime);
        }

        protected override void Loot()
        {
            //provide the player with a random loot fom the data var
            //later on, we'd need to roll for a rarity, but rn we'll just pick a random item
            int i = Random.Range(0, data.lootables.Length);

            //if we have the player inventory reference, add the item to their inventory.
            //if (playerInventory) playerInventory.AddInventoryItem(data.lootables[i]);

            //we're gonna need to find a clear slot, or a slot with the same item
            //and we're gonna need to compare the slot type to the item type
            //e.g. WEAPON, TOOL, CONSUMABLE

            EndLoot();
        }

        protected override void EndLoot()
        {
            Debug.Log("Ended Looting the Consumable Containter!");

            isLooting = false;
        }

        public override void StopLoot()
        {
            EndLoot();

            Debug.Log("Stopped Looting the Consumable Containter!");

            CancelInvoke(nameof(Loot));
        }
    }
}