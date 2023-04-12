using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "ConsumableData", menuName = "Items/Consumable")]
    public class ConsumableData : ItemData
    {
        [Header("Attributes - Consumable")]
        public ConsumableType consumableType;

        [Space]
        [Tooltip("How many times you can Consume this item")] public int useAmount;
        [Tooltip("The amount of a stat that is replenished per use of this item")] public int consumableAmount;
        [Tooltip("How long it takes to Consume this item, per amount")] public float consumeTime;
    }
}