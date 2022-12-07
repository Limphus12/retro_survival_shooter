using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Items/Item")]
    public class ItemData : ScriptableObject
    {
        [Header("Attributes - Item")]
        public GameObject model;

        [Space]
        public string itemName;
        public double itemWeight;
    }
}