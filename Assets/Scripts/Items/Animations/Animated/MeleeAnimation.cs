using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class MeleeAnimation : ItemAnimation
    {
        const string MELEE_IDLE = "melee_idle";
        const string MELEE_BLOCK = "melee_block";
        const string MELEE_BLOCK_HIT = "melee_block_hit";
        const string MELEE_LIGHT_ATTACK = "melee_light_attack";
        const string MELEE_CHARGE_ATTACK = "melee_charge_attack";
        const string MELEE_HEAVY_ATTACK = "melee_heavy_attack";
        const string MELEE_EXHAUSTED_ATTACK = "melee_exhausted_attack";

        public void PlayMeleeIdle() => PlayAnimation(MELEE_IDLE);
        public void PlayMeleeBlock() => PlayAnimation(MELEE_BLOCK);
        public void PlayMeleeBlockHit() => PlayAnimation(MELEE_BLOCK_HIT);
        public void PlayMeleeLightAttack() => PlayAnimation(MELEE_LIGHT_ATTACK);
        public void PlayMeleeChargeAttack() => PlayAnimation(MELEE_CHARGE_ATTACK);
        public void PlayMeleeHeavyAttack() => PlayAnimation(MELEE_HEAVY_ATTACK);
        public void PlayMeleeExhaustedAttack() => PlayAnimation(MELEE_EXHAUSTED_ATTACK);
    }
}