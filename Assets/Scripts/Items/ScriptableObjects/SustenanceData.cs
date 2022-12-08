using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(fileName = "SustenanceData", menuName = "Items/Consumable/Sustenance")]

    [System.Serializable]
    public class SustenanceData : ConsumableData
    {
        [Header("Attributes - Sustenance")]
        public SustenanceType sustenanceType;

        [Tooltip("The total amount of sustenace that can be consumed from this")] public int sustenanceAmount;
    }
}