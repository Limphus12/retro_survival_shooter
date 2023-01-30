using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class MeleeSound : ItemSound
    {
        [Header("Attributes - Melee Sound")]
        [SerializeField] protected MeleeSoundData meleeSoundData;

        [Space]
        [SerializeField] protected AudioClip lightAttackClip;
        [SerializeField] protected AudioClip heavyAttackClip, exhaustedAttackClip;

        [Space]
        [SerializeField] protected AudioClip lightAttackHitClip;
        [SerializeField] protected AudioClip heavyAttackHitClip, exhaustedAttackHitClip;

        [Space]
        [SerializeField] protected AudioClip blockHitClip;

        protected override void Init()
        {
            if (!meleeSoundData)
            {
                Debug.LogWarning("No Melee Sound Data found for " + gameObject.name + "; Assign Melee Sound Data!");
                return;
            }

            equipClip = meleeSoundData.equipClip;
            audioMixerGroup = meleeSoundData.audioMixerGroup;

            lightAttackClip = meleeSoundData.lightAttackClip;
            heavyAttackClip = meleeSoundData.heavyAttackClip;
            exhaustedAttackClip = meleeSoundData.exhaustedAttackClip;

            lightAttackHitClip = meleeSoundData.lightAttackHitClip;
            heavyAttackHitClip = meleeSoundData.heavyAttackHitClip;
            exhaustedAttackHitClip = meleeSoundData.exhaustedAttackHitClip;

            blockHitClip = meleeSoundData.blockHitClip;
        }

        protected override void Start() => PlayEquipSound();

        public void PlayEquipSound() => PlaySound(equipClip);
        public void PlayLightAttackSound() => PlaySound(lightAttackClip);
        public void PlayHeavyAttackSound() => PlaySound(heavyAttackClip);
        public void PlayExhaustedAttackSound() => PlaySound(exhaustedAttackClip);

        public void PlayLightAttackHitSound() => PlaySound(lightAttackHitClip);
        public void PlayHeavyAttackHitSound() => PlaySound(heavyAttackHitClip);
        public void PlayExhaustedAttackHitSound() => PlaySound(exhaustedAttackHitClip);

        public void PlayBlockHitSound() => PlaySound(blockHitClip);
    }
}