using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class AmmoContainer : Container
    {
        [Header("Attributes - Ammo")]
        [SerializeField] private float ammoPercentage; //A certain percent of ammo that is restored

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


            EndLoot();
        }

        protected override void EndLoot()
        {
            isLooting = false;
        }
    }
}