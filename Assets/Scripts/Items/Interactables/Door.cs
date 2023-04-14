using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class Door : InteractableItem
    {

        [Header("Attributes")]
        [SerializeField] private Vector3 closedRotation;
        [SerializeField] private Vector3 openRotation;

        [Space, SerializeField] private float rotationSmooth;

        private bool isOpen, isRotating;

        protected Quaternion initialRotation;

        private void Awake() => Init();

        private void Init()
        {
            initialRotation = transform.rotation;
        }

        private void Update()
        {
            if (isRotating)
            {
                if (isOpen) Rotate(openRotation);
                else if (!isOpen) Rotate(closedRotation);
            }
        }

        public override bool CanInteract() => true;

        public override void Interact()
        {
            isOpen = !isOpen; StartRotate();
        }

        void StartRotate()
        {
            rotateI = 0;
            isRotating = true;
        }

        void EndRotate()
        {
            rotateI = 0;
            isRotating = false;
        }

        float rotateI = 0;

        void Rotate(Vector3 rotation)
        {
            rotateI += Time.deltaTime * rotationSmooth;

            //apply target rotation
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(rotation), rotateI);

            if (rotateI >= 1)
            {
                EndRotate();
            }
        }
    }
}