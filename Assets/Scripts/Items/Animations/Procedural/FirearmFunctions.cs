using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class FirearmFunctions : MonoBehaviour
    {
        [SerializeField] private Transform hammer;

        [Space]
        [SerializeField] private Vector3 cockedPosition;
        [SerializeField] private Vector3 cockedRotation;

        [SerializeField] private float cockedSmoothRate;

        [Space]
        [SerializeField] private Vector3 unCockedPosition;
        [SerializeField] private Vector3 unCockedRotation;

        [SerializeField] private float unCockedSmoothRate;

        private Vector3 targetPosition;
        private Vector3 targetRotation;
        private float targetSmooth;

        private void Update()
        {
            LerpPosition(targetPosition, targetSmooth);
            LerpRotation(targetRotation, targetSmooth);
        }

        public void Cock()
        {
            targetPosition = cockedPosition; targetRotation = cockedRotation; targetSmooth = cockedSmoothRate;
        }

        public void UnCock()
        {
            targetPosition = unCockedPosition; targetRotation = unCockedRotation; targetSmooth = unCockedSmoothRate;
        }

        private void LerpPosition(Vector3 position, float smooth)
        {
            hammer.localPosition = Vector3.Lerp(transform.localPosition, position, smooth * Time.deltaTime);
        }

        private void LerpRotation(Vector3 rotation, float smooth)
        {
            hammer.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rotation), smooth * Time.deltaTime);
        }

    }
}