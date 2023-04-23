using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.limphus.retro_survival_shooter
{
    public class MainMenu : Menu
    {
        [SerializeField] private GameObject[] mainMenuUI, optionsUI, audioUI, videoUI, controlsUI, playUI, newUI, loadingUI;

        [Space]
        [SerializeField] private int playSceneIndex;

        private void Start()
        {
            //We'll set up the mouse to be visible, and confined
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        public void Play()
        {
            SceneManager.LoadSceneAsync(playSceneIndex);
        }

        public void StartButton()
        {
            ToggleUI(playUI, mainMenuUI);
        }

        public void PlayBackButton()
        {
            ToggleUI(mainMenuUI, playUI);
        }

        public void NewButton()
        {
            ToggleUI(newUI, playUI);
        }

        public void NewBackButton()
        {
            ToggleUI(playUI, newUI);
        }

        public void Loading()
        {
            ToggleUI(loadingUI, newUI);
        }

        public void OptionsButton()
        {
            ToggleUI(optionsUI, mainMenuUI);
        }

        public void OptionsBackButton()
        {
            ToggleUI(mainMenuUI, optionsUI);

            TurnOffUI(audioUI);
            TurnOffUI(videoUI);
            TurnOffUI(controlsUI);
        }

        public void AudioOptionsButton()
        {
            TurnOnUI(audioUI);

            TurnOffUI(videoUI);
            TurnOffUI(controlsUI);
        }

        public void VideoOptionsButton()
        {
            TurnOnUI(videoUI);

            TurnOffUI(audioUI);
            TurnOffUI(controlsUI);
        }

        public void ControlsOptionButton()
        {
            TurnOnUI(controlsUI);

            TurnOffUI(audioUI);
            TurnOffUI(videoUI);
        }

        public void QuitButton()
        {
            Application.Quit();
        }
    }
}