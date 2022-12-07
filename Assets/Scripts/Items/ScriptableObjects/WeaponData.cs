using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Items/Weapon")]
    public class WeaponData : ScriptableObject
    {
        [Header("Attributes - Weapon")]
        public int damage;
        public float rateOfFire;
    }
}