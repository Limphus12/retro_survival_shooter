using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class WeaponSway : MonoBehaviour
    {
        [Header("Hipfire Settings")]
        [SerializeField] private Vector3 hipPosition;
        [SerializeField] private Quaternion hipRotation;

        [Space]
        [SerializeField] private float hipSwaySmooth = 8.0f;
        [SerializeField] private float hipSwayAmount = 2.0f, hipSwayMaximum = 2.0f;

        [Space]
        [SerializeField] private float hipTiltSmooth = 8.0f;
        [SerializeField] private float hipTiltAmount = 2.0f, hipTiltMaximum = 2.0f;

        [Header("Aiming Settings")]
        [SerializeField] private Vector3 aimingPosition;
        [SerializeField] private Quaternion aimingRotation;

        [Space]
        [SerializeField] private float aimingSwaySmooth = 8.0f;
        [SerializeField] private float aimingSwayAmount = 2.0f, aimingSwayMaximum = 2.0f;

        [Space]
        [SerializeField] private float aimingTiltSmooth = 8.0f;
        [SerializeField] private float aimingTiltAmount = 2.0f, aimingTiltMaximum = 2.0f;

        [Header("Reloading Settings")]
        [SerializeField] private Vector3 reloadingPosition;
        [SerializeField] private Quaternion reloadingRotation;

        [Space]
        [SerializeField] private float reloadingSwaySmooth = 8.0f;
        [SerializeField] private float reloadingSwayAmount = 2.0f, reloadingSwayMaximum = 2.0f;

        [Space]
        [SerializeField] private float reloadingTiltSmooth = 8.0f;
        [SerializeField] private float reloadingTiltAmount = 2.0f, reloadingTiltMaximum = 2.0f;

        private Quaternion initialRotation;

        private bool isAiming, isReloading;

        private void Awake()
        {
            initialRotation = transform.localRotation;
        }

        void Update()
        {
            CheckSway();
            CheckTilt();
        }

        public void Aim(bool b) => isAiming = b;

        public void Reload(bool b) => isReloading = b;

        private void CheckSway()
        {
            if (isReloading) Sway(reloadingSwayAmount, reloadingSwayMaximum, reloadingSwaySmooth, reloadingPosition);

            else if (!isAiming) Sway(hipSwayAmount, hipSwayMaximum, hipSwaySmooth, hipPosition);

            else if (isAiming) Sway(aimingSwayAmount, aimingSwayMaximum, aimingSwaySmooth, aimingPosition);
        }

        private void CheckTilt()
        {
            if (isReloading) Tilt(reloadingTiltAmount, reloadingTiltMaximum, reloadingTiltSmooth, reloadingRotation);

            else if (!isAiming) Tilt(hipTiltAmount, hipTiltMaximum, hipTiltSmooth, hipRotation);

            else if (isAiming) Tilt(aimingTiltAmount, aimingTiltMaximum, aimingTiltSmooth, aimingRotation);
        }

        private void Sway(float amount, float maximum, float smooth, Vector3 position)
        {
            //calculate inputs
            Vector2 inputs = Inputs();

            //calculate sway positions
            float swayPositionX = Mathf.Clamp(inputs.x * amount, -maximum , maximum) / 100;
            float swayPositionY = Mathf.Clamp(inputs.y * amount, -maximum, maximum) / 100;

            //calculate final target position
            Vector3 targetPosition = new Vector3(swayPositionX, swayPositionY);

            //apply target position
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition + position, smooth * Time.deltaTime);
        }

        private void Tilt(float amount, float maximum, float smooth, Quaternion rotation)
        {
            //calculate inputs
            Vector2 inputs = Inputs();

            //calculate tilt rotations
            float tiltRotationY = Mathf.Clamp(inputs.x * amount, -maximum, maximum);
            float tiltRotationX = Mathf.Clamp(inputs.y * amount, -maximum, maximum);
            float tiltRotationZ = Mathf.Clamp(Input.GetAxis("Horizontal") * amount, -maximum, maximum);

            //calculate target rotation
            Quaternion tiltRotation = Quaternion.Euler(new Vector3(tiltRotationX, -tiltRotationY, -tiltRotationZ));

            Quaternion targetRotation = rotation * tiltRotation;

            //apply target rotation
            //transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation * initialRotation, smooth * Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation * initialRotation, smooth * Time.deltaTime);
        }

        private Vector2 Inputs()
        {
            Vector2 inputs = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            return inputs;
        }
    }
}