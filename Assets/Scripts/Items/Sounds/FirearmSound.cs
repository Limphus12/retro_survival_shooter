using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class FirearmSound : ItemSound
    {
        [Header("Attributes - Firearm Sound")]
        [SerializeField] protected FirearmSoundData firearmSoundData;

        [Space]
        [SerializeField] protected AudioClip firingClip;
        [SerializeField] protected AudioClip dryFiringClip, reloadingClip;

        [Space]
        [SerializeField] protected AudioClip cockingClip;

        protected override void Init()
        {
            if (!firearmSoundData)
            {
                Debug.LogWarning("No Firearm Sound Data found for " + gameObject.name + "; Assign Firearm Sound Data!");
                return;
            }

            equipClip = firearmSoundData.equipClip;
            audioMixerGroup = firearmSoundData.audioMixerGroup;

            firingClip = firearmSoundData.firingClip;
            reloadingClip = firearmSoundData.reloadingClip;

            dryFiringClip = firearmSoundData.dryFiringClip;

            cockingClip = firearmSoundData.cockingClip;
        }

        public void PlayFiringSound() => PlaySound(firingClip);
        public void PlayReloadingSound() => PlaySound(reloadingClip);
        public void PlayDryFiringSound() => PlaySound(dryFiringClip);
        public void PlayCockingSound() => PlaySound(cockingClip);
    }
}