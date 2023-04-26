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
        public float lightAttackTimeToHit;
        public int lightAttackStaminaCost, lightAttackDamage;

        [Space]
        public float heavyAttackRate;
        public float chargeUpTime, heavyAttackTimeToHit;
        public int heavyAttackStaminaCost, heavyAttackDamage;

        [Space]
        public float exhaustedAttackRate;
        public float exhaustedAttackTimeToHit;
        public int exhaustedAttackDamage;

    }
}