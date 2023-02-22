using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class AmmoContainer : Container
    {
        [Header("Attributes - Ammo")]
        [SerializeField] private float ammoPercentage; //A certain percent of ammo that is restored

        public override void StartInteract()
        {
            isInteracting = true;
        }

        public override void StopInteract()
        {
            isInteracting = false;
        }

        public override bool IsLooting() => isLooting;

        public override void StartLoot()
        {
            isLooting = true;

            Debug.Log("Started Looting the Ammo Containter!");

            Invoke(nameof(Loot), lootTime);
        }

        protected override void Loot()
        {
            //if we have the player inventory reference - if (playerInventory)
            //check through all of the slots so that we can find any weapons
            //specificaly firearms

            //then we can call a method on them (to be implemented)
            //so that we can replenish a certain percentage of ammo
            //we'll need GetAmmo and SetAmmo functions
            //and we'll also need to calc the actual % of ammo using those functions.

            //essentially the ammo will be a 'stat' of the firearm itself.
            //similar to the player stats...

            EndLoot();
        }

        protected override void EndLoot()
        {
            Debug.Log("Ended Looting the Ammo Containter!");

            isLooting = false;
        }

        public override void StopLoot()
        {
            EndLoot();

            Debug.Log("Stopped Looting the Ammo Containter!");

            CancelInvoke(nameof(Loot));
        }
    }
}