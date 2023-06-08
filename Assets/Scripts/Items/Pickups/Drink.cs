using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter.items
{
    public class Drink : Pickup
    {
        protected override void PickUp()
        {
            if (GameManager.PlayerStats.CanReplenishThirst())
            {
                GameManager.PlayerStats.ReplenishThirst(amount);

                Destroy();
            }
        }
    }
}