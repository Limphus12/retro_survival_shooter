using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class Menu : MonoBehaviour
    {
        protected void ToggleUI(GameObject[] ui, bool toggle)
        {
            foreach (GameObject UI in ui)
            {
                UI.SetActive(toggle);
            }
        }

        protected void ToggleUI(GameObject[] toggleOn, GameObject[] toggleOff)
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

        protected void TurnOnUI(GameObject[] ui)
        {
            foreach (GameObject UI in ui)
            {
                UI.SetActive(true);
            }
        }

        protected void TurnOffUI(GameObject[] ui)
        {
            foreach (GameObject UI in ui)
            {
                UI.SetActive(false);
            }
        }
    }
}