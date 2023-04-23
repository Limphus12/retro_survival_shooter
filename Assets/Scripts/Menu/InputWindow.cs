using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class InputWindow : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;

        [SerializeField] private string validCharacters;

        private void Start()
        {
            inputField.onValidateInput = (string text, int charIndex, char addedChar) =>
            {
                return ValidateChar(validCharacters, addedChar);
            };
        }

        public void SubmitInput()
        {
            if (inputField.text == "")
            {
                WorldGenerator.SetCurrentSeed(UnityEngine.Random.Range(-32766, 32766)); //16 bit int limit
            }

            else WorldGenerator.SetCurrentSeed(int.Parse(inputField.text));
        }

        private char ValidateChar(string validCharacters, char addedChar)
        {
            if (validCharacters.IndexOf(addedChar) != -1)
            {
                return addedChar;
            }

            else return '\0';
        }
    }
}