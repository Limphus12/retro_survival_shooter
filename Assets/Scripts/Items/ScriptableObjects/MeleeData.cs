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
        public float attackRange;

        [Space]
        public float lightAttackTimeToHit;

        [Space]
        public float heavyAttackRate;
        public float heavyAttackDamage, chargeUpTime, heavyAttackTimeToHit;

        [Space]
        public float exhaustedAttackRate;
        public float exhaustedAttackDamage, exhaustedAttackTimeToHit;
    }
}