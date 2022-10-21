using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class WeaponSway : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float swaySmooth = 8.0f;
        [SerializeField] private float swayAmount = 2.0f, swayMaximum = 2.0f;
        
        [Space]
        [SerializeField] private float tiltSmooth = 8.0f;
        [SerializeField] private float tiltAmount = 2.0f, tiltMaximum = 2.0f;

        private Vector3 initialPosition;
        private Quaternion initialRotation;

        private void Awake()
        {
            initialPosition = transform.localPosition;
            initialRotation = transform.localRotation;
        }

        void Update()
        {
            Sway();
            Tilt();
        }

        private void Sway()
        {
            //calculate inputs
            Vector2 inputs = Inputs();

            //calculate sway positions
            float swayPositionX = Mathf.Clamp(inputs.x * swayAmount, -swayMaximum , swayMaximum) / 100;
            float swayPositionY = Mathf.Clamp(inputs.y * swayAmount, -swayMaximum, swayMaximum) / 100;

            //calculate final target position
            Vector3 targetPosition = new Vector3(swayPositionX, swayPositionY);

            //apply target position
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition + initialPosition, swaySmooth * Time.deltaTime);
        }

        private void Tilt()
        {   
            //calculate inputs
            Vector2 inputs = Inputs();

            //calculate tilt rotations
            float tiltRotationY = Mathf.Clamp(inputs.x * tiltAmount, -tiltMaximum, tiltMaximum);
            float tiltRotationX = Mathf.Clamp(inputs.y * tiltAmount, -tiltMaximum, tiltMaximum);

            //calculate target rotation
            Quaternion targetRotation = Quaternion.Euler(new Vector3(-tiltRotationX, tiltRotationY, tiltRotationY));

            //apply target rotation
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation * initialRotation, tiltSmooth * Time.deltaTime);
        }

        private Vector2 Inputs()
        {
            Vector2 inputs = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            return inputs;
        }
    }
}