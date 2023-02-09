using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [CreateAssetMenu(fileName = "MeleeData", menuName = "Items/Melee")]

    [System.Serializable]
    public class MeleeData : ItemData
    {
        [Header("Attributes - Melee")]
        public float attackRange;

        [Space]
        public float lightAttackRate;
        public float lightAttackDamage, lightAttackTimeToHit;
        public int lightAttackStaminaCost;

        [Space]
        public float heavyAttackRate;
        public float heavyAttackDamage, chargeUpTime, heavyAttackTimeToHit;
        public int heavyAttackStaminaCost;

        [Space]
        public float exhaustedAttackRate;
        public float exhaustedAttackDamage, exhaustedAttackTimeToHit;

    }
}