using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    //[CreateAssetMenu(fileName = "ConsumableData", menuName = "Items/Consumable")]

    [System.Serializable]
    public class ConsumableData : ItemData
    {
        [Header("Attributes - Consumable")]
        [Tooltip("How many times you can Consume this item")] public int consumeAmount;
        [Tooltip("How long it takes to Consume this item")] public float consumeTime;
    }
}