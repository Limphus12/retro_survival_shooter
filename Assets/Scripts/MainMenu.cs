using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject[] mainMenuUI, optionsUI;

        private void Start()
        {
            //We'll set up the mouse to be visible, and confined
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        public void StartButton()
        {
            Debug.Log("Start Button");
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
    }
}