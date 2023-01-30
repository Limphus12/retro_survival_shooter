using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    //[CreateAssetMenu(fileName = "WeaponData", menuName = "Items/Weapon")]

    [System.Serializable]
    public class WeaponData : ItemData
    {
        [Header("Attributes - Weapon")]
        public int damage;
        public float attackRate;
    }
}