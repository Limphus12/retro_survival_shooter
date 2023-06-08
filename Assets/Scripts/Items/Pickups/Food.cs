using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter.items
{
    public class Food : Pickup
    {
        protected override void PickUp()
        {
            if (GameManager.PlayerStats.CanReplenishHunger())
            {
                GameManager.PlayerStats.ReplenishHunger(amount);

                Destroy();
            }
        }
    }
}