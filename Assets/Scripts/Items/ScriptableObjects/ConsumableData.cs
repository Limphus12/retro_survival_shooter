using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "ConsumableData", menuName = "Items/Consumable")]
    public class ConsumableData : MeleeData
    {
        [Header("Attributes - Consumable")]
        public ConsumableType consumableType;

        [Space]
        [Tooltip("How many times you can Consume this item")] public int useAmount;
        [Tooltip("The total amount of a stat that can be replenished from this")] public int consumableAmount;
        [Tooltip("How long it takes to Consume this item, per amount")] public float consumeTime;
    }
}