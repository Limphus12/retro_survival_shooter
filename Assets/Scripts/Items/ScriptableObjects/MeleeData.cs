using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(fileName = "MeleeData", menuName = "Items/Weapon/Melee")]

    [System.Serializable]
    public class MeleeData : WeaponData
    {
        [Header("Attributes - Melee")]
        public float meleeRange;
        public float timeToHit;
    }
}