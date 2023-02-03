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
        }

        public void PlayConsumeSound() => PlaySound(consumeClip);
    }
}