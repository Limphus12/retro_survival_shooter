using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace com.limphus.retro_survival_shooter
{
    public class ItemSound : MonoBehaviour
    {
        [Header("Attributes - Item Sound")]
        [SerializeField] protected ItemSoundData itemSoundData;

        [Space]
        [SerializeField] protected AudioClip equipClip;

        [Space]
        [SerializeField] protected AudioMixerGroup audioMixerGroup;

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

        protected void PlaySound(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning("No Audio Clip found! Assign an Audio Clip in the Sound Data!");
                return;
            }

            //instance of an object, attach an audio source & play our clip!
            GameObject soundGameObject = new GameObject("Sound");
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();

            //assign to the correct audio mixer and play!
            audioSource.outputAudioMixerGroup = audioMixerGroup;
            audioSource.PlayOneShot(clip);
            audioSource.spatialBlend = 1f; //3D

            //oh and make sure we're placed wherever this object is.
            soundGameObject.transform.position = gameObject.transform.position;

            //make sure to destroy the sound game object after the sound is done!
            Destroy(soundGameObject, clip.length);
        }
    }
}