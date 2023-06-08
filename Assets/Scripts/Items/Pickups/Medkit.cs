using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class Medkit : Pickup
    {
        protected override void PickUp()
        {
            if (GameManager.PlayerStats.CanReplenishHealth())
            {
                GameManager.PlayerStats.ReplenishHealth(pickupAmount);

                Destroy();
            }
        }
    }
}