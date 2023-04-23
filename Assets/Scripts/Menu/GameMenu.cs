using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.limphus.retro_survival_shooter
{
    public class GameMenu : Menu
    {
        [SerializeField] private GameObject[] menuUI, gameUI;

        [Space]
        [SerializeField] private int menuSceneIndex = 0;

        private bool toggleMenu = false;

        private void Start()
        {
            toggleMenu = false;

            ToggleFunctionality();
        }

        private void Update() => CheckInput();

        private void CheckInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                toggleMenu = !toggleMenu;

                ToggleUI(menuUI, toggleMenu);
                ToggleUI(gameUI, !toggleMenu);

                ToggleFunctionality();
            }
        }

        private void ToggleFunctionality()
        {
            PlayerController.canMove = !toggleMenu;
            PlayerController.canRotate = !toggleMenu;
            PlayerController.canCameraLean = !toggleMenu;

            Item.CanUse = !toggleMenu;

            Cursor.visible = toggleMenu;

            if (toggleMenu) Cursor.lockState = CursorLockMode.Confined;
            else if (!toggleMenu) Cursor.lockState = CursorLockMode.Locked;
        }

        public void MainMenuButton()
        {
            SceneManager.LoadSceneAsync(menuSceneIndex);
        }
    }
}