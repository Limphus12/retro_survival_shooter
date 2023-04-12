using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class ConsumableContainer : Container
    {
        [Header("Attributes - Consumable")]
        [SerializeField] private ConsumableContainerData data;

        [SerializeField] private List <Consumable> consumables = new List<Consumable>();

        protected override void Init()
        {
            base.Init();
        }

        private void Start()
        {
            consumables = playerInventory.GetConsumables();
        }

        //checking if we can loot this container
        public override bool CanLoot()
        {
            consumables = playerInventory.GetConsumables();

            if (remainingLootAmount > 0 && consumables.Count > 0)
            {
                bool b = false;

                foreach(Consumable consumable in consumables)
                {
                    if (consumable.CanReplenish()) b = true;
                }

                return b;
            }

            else return false;
        }

        public override void Interact() => Loot();

        protected override void Loot()
        {
            //check if we have the player inventory and that we can
            if (playerInventory && consumables.Count > 0)
            {
                //pick a random consumable to replenish by 1
                int i = Random.Range(0, consumables.Count);

                consumables[i].Replenish(1);
            }

            remainingLootAmount--;
        }
    }
}