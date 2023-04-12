using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class AmmoContainer : Container
    {
        [Header("Attributes - Ammo")]
        [Range(0f, 1f), SerializeField] private float ammoPercentage; //A certain percent of ammo that is restored

        public override bool CanLoot()
        {
            if (remainingLootAmount > 0) return true;
            else return false;
        }

        public override void Interact() => Loot();

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

            if (playerInventory)
            {
                List<Firearm> firearms = playerInventory.GetFirearms();

                if (firearms != null)
                {
                    foreach(Firearm firearm in firearms)
                    {
                        //TODO! CURRENTLY WE CANNOT GET THE AMMO RESERVES!

                        //calculate how much ammo we need to replenish

                        //by getting the max ammo reserves, and multiplying it by our ammo percentage
                        //and rounding it to the nearest int
                        //int k = Mathf.RoundToInt(firearm.GetMaxAmmoReserves() * ammoPercentage);

                        //then set our current ammo reserves
                        //firearm.SetCurrentAmmoReserves(firearm.GetCurrentAmmoReserves() + k);
                    }
                }
            }
        }
    }
}