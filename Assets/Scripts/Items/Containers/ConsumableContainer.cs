using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class ConsumableContainer : Container
    {
        [Header("Attributes - Consumable")]
        [SerializeField] private ConsumableContainerData data;

        public override void StartInteract()
        {
            isInteracting = true;

            if (containerAnimation) containerAnimation.PlayLooting();
            if (containerSound) containerSound.PlayLootingSound();
        }

        public override void StopInteract()
        {
            isInteracting = false;

            if (containerAnimation) containerAnimation.PlayIdle();
            if (containerSound) containerSound.PlayLootingStopSound();
        }

        public override bool CanLoot()
        {
            if (remainingLootAmount > 0) return true;

            else return false;
        }

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

            //check if we have the player inventory and that we can add the item
            if (playerInventory && playerInventory.CanAddItem(GetItemType()))
            {
                if (playerInventory.CanAddItem(GetItemType()))
                {
                    int i = Random.Range(0, data.lootables.Length);

                    playerInventory.AddItem(Instantiate(data.lootables[i]), GetItemType());
                }

                else
                {
                    Debug.Log("No Inventory Space!");
                    EndLoot(); return;
                }
            }

            remainingLootAmount--;

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