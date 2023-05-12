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

        [Space, SerializeField] private bool isPlayer;

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

        public void PlayFiringSound()
        {
            if (!isPlayer) { PlaySound(firingClip, transform.position); return; }
            PlaySound(firingClip);
        }
        public void PlayReloadingSound()
        {
            if (!isPlayer) { PlaySound(reloadingClip, transform.position); return; }
            PlaySound(reloadingClip);
        }
        public void PlayDryFiringSound()
        {
            if (!isPlayer) { PlaySound(dryFiringClip, transform.position); return; }
            PlaySound(dryFiringClip);
        }
        public void PlayCockingSound()
        {
            if (!isPlayer) { PlaySound(cockingClip, transform.position); return; }
            PlaySound(cockingClip);
        }
    }
}