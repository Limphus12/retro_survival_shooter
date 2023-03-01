using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class ItemSound : SoundHandler
    {
        [Header("Attributes - Item Sound")]
        [SerializeField] protected ItemSoundData itemSoundData;

        [Space]
        [SerializeField] protected AudioClip equipClip;

        private void Awake() => Init();

        protected virtual void Init()
        {
            if (!itemSoundData)
            {
                Debug.LogWarning("No Item Sound Data found for " + gameObject.name + "; Assign Item Sound Data!");
                return;
            }

            equipClip = itemSoundData.equipClip;
            audioMixerGroup = itemSoundData.audioMixerGroup;
        }

        public void PlayEquipSound() => PlaySound(equipClip);

        public virtual ItemSoundData GetItemSoundData()
        {
            if (itemSoundData != null) return itemSoundData;

            else return null;
        }

        public virtual void SetItemSoundData(ItemSoundData itemSoundData)
        {
            this.itemSoundData = itemSoundData;

            Init();
        }
    }
}