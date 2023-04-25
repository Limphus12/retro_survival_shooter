using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

namespace com.limphus.retro_survival_shooter
{
    public class Settings : MonoBehaviour
    {
        [Header("Video - Dropdown")]
        [SerializeField] private TMP_Dropdown resolutionDropdown;

        [Header("Audio - Mixer")]
        [SerializeField] private AudioMixer masterMixer;

        [Header("Audio - Sliders")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider ambienceVolumeSlider, soundVolumeSlider, UIScaleSlider;

        private Resolution[] resolutions;

        public static float currentMasterVolume, currentAmbienceVolume, currentSoundVolume;

        public static Resolution currentResolution;

        public static bool fullscreen;

        public static int currentQualityLevel;

        private void Start()
        {
            //resolutions
            resolutions = Screen.resolutions;

            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();

            int currentRes = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                {
                    currentRes = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentRes;
            resolutionDropdown.RefreshShownValue();

            //mixer stuff
            if (masterMixer)
            {
                if (masterVolumeSlider)
                {
                    masterVolumeSlider.value = currentMasterVolume;

                    masterMixer.SetFloat("MasterVolume", currentMasterVolume);
                }

                if (ambienceVolumeSlider)
                {
                    ambienceVolumeSlider.value = currentAmbienceVolume;

                    masterMixer.SetFloat("AmbienceVolume", currentAmbienceVolume);
                }

                if (soundVolumeSlider)
                {
                    soundVolumeSlider.value = currentSoundVolume;

                    masterMixer.SetFloat("SoundVolume", currentSoundVolume);
                }
            }

            if (UIScaleSlider) UIScaleSlider.value = UIManager.UIScale;
        }

        //set the master volume
        public void SetMasterVolume(float i)
        {
            if (masterMixer)
            {
                currentMasterVolume = i;

                masterMixer.SetFloat("MasterVolume", currentMasterVolume);
            }
        }

        //set the ambience volume
        public void SetAmbienceVolume(float i)
        {
            if (masterMixer)
            {
                currentAmbienceVolume = i;

                masterMixer.SetFloat("AmbienceVolume", currentAmbienceVolume);
            }
        }

        //set the sound volume
        public void SetSoundVolume(float i)
        {
            if (masterMixer)
            {
                currentSoundVolume = i;

                masterMixer.SetFloat("SoundVolume", currentSoundVolume);
            }
        }

        public void SetQuality(int i)
        {
            currentQualityLevel = i;
            QualitySettings.SetQualityLevel(i);
        }

        public void SetFullscreen(bool i)
        {
            fullscreen = i;
            Screen.fullScreen = i;
        }

        public void SetResolution(int i)
        {
            Resolution resolution = resolutions[i];
            currentResolution = resolutions[i];
            Screen.SetResolution(resolution.width, resolution.height, fullscreen);
        }

        public void SetUIScale(float i)
        {
            UIManager.UIScale = i;
        }
    }
}