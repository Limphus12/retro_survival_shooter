using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Items/Item")]

    [System.Serializable]
    public class ItemData : ScriptableObject
    {
        [Header("Attributes - Item")]
        public GameObject prefab;

        [Space]
        public string itemName;
        public double itemWeight;
    }
}