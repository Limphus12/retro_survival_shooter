using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class ConsumableSound : ItemSound
    {
        [Header("Attributes - Firearm Sound")]
        [SerializeField] protected SustenanceSoundData sustenanceSoundData;

        [Space]
        [SerializeField] protected AudioClip consumeClip;
        [SerializeField] protected AudioClip consumingClip;

        protected override void Init()
        {
            if (!sustenanceSoundData)
            {
                Debug.LogWarning("No Sustenance Sound Data found for " + gameObject.name + "; Assign Sustenance Sound Data!");
                return;
            }

            equipClip = sustenanceSoundData.equipClip;
            audioMixerGroup = sustenanceSoundData.audioMixerGroup;

            consumeClip = sustenanceSoundData.consumeClip;
            consumingClip = sustenanceSoundData.consumingClip;
        }

        public void PlayConsumeSound() => PlaySound(consumeClip);
        public void PlayConsumingSound() => PlaySound(consumingClip);
    }
}