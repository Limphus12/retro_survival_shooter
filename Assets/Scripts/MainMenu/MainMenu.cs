using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject[] mainMenuUI, optionsUI, audioUI, videoUI, playUI, newUI;

        private void Start()
        {
            //We'll set up the mouse to be visible, and confined
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        public void StartButton()
        {
            Debug.Log("Start Button");

            ToggleUI(playUI, mainMenuUI);
        }

        public void PlayBackButton()
        {
            Debug.Log("Play Back Button");

            ToggleUI(mainMenuUI, playUI);
        }

        public void NewButton()
        {
            Debug.Log("New Button");

            ToggleUI(newUI, playUI);
        }

        public void NewBackButton()
        {
            Debug.Log("New Back Button");

            ToggleUI(playUI, newUI);
        }

        public void OptionsButton()
        {
            Debug.Log("Options Button");

            ToggleUI(optionsUI, mainMenuUI);
        }

        public void OptionsBackButton()
        {
            Debug.Log("Options Back Button");

            ToggleUI(mainMenuUI, optionsUI);

            TurnOffUI(audioUI);
            TurnOffUI(videoUI);
        }

        public void AudioOptionsButton()
        {
            Debug.Log("Audio Options Button");

            TurnOnUI(audioUI);

            TurnOffUI(videoUI);
        }

        public void VideoOptionsButton()
        {
            Debug.Log("Video Options Button");

            TurnOnUI(videoUI);

            TurnOffUI(audioUI);
        }

        public void QuitButton()
        {
            Debug.Log("Quit Button");

            Application.Quit();
        }

        private void ToggleUI(GameObject[] toggleOn, GameObject[] toggleOff)
        {
            foreach (GameObject UI in toggleOn)
            {
                UI.SetActive(true);
            }

            foreach (GameObject UI in toggleOff)
            {
                UI.SetActive(false);
            }
        }

        private void TurnOnUI(GameObject[] ui)
        {
            foreach (GameObject UI in ui)
            {
                UI.SetActive(true);
            }
        }

        private void TurnOffUI(GameObject[] ui)
        {
            foreach (GameObject UI in ui)
            {
                UI.SetActive(false);
            }
        }
    }
}