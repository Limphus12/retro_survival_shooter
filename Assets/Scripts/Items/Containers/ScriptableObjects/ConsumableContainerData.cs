using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{

    [CreateAssetMenu(fileName = "ConsumableContainerData", menuName = "Containers/Consumable")]
    public class ConsumableContainerData : ScriptableObject
    {
        [Header("Attributes - Consumable Loot")]
        public GameObject[] lootables;

        //later on, add a rarity system?
    }
}
