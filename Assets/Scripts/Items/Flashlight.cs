using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class Flashlight : MonoBehaviour, IUseable
    {
        [SerializeField] private GameObject toggleObject;

        [SerializeField] private KeyCode toggleKey = KeyCode.F;
        [SerializeField] private int toggleMouseButton = 0;

        bool toggle;

        public bool DetectKeyboardInput(KeyCode key)
        {
            if (Input.GetKeyDown(key)) return true;
            else return false;
        }

        public bool DetectMouseInput(int button)
        {
            if (Input.GetMouseButtonDown(button)) return true;
            else return false;
        }

        public void CheckInput()
        {
            if (DetectMouseInput(toggleMouseButton) || DetectKeyboardInput(toggleKey)) ToggleFlashlight();
        }

        private void ToggleFlashlight()
        {
            toggle = !toggle; toggleObject.SetActive(toggle);
        }

    }
}