using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class Door : InteractableItem
    {
        [Header("Attributes")]
        [SerializeField] private Vector3 closedPosition;
        [SerializeField] private Vector3 openPosition;
        
        [Space, SerializeField] private float positionSmooth;

        [Space]
        [SerializeField] private Vector3 closedRotation;
        [SerializeField] private Vector3 openRotation;

        [Space, SerializeField] private float rotationSmooth;

        private bool isOpen, isRotating, isMoving;

        private void Awake()
        {
            int i = Random.Range(0, 5);

            if (i == 0) isOpen = true;
            else isOpen = false;

            if (isOpen)
            {
                if (positionSmooth > 0) transform.localPosition = openPosition;
                if (rotationSmooth > 0) transform.localRotation = Quaternion.Euler(openRotation);
            }

            else if (!isOpen)
            {
               if (positionSmooth > 0) transform.localPosition = closedPosition;
               if (rotationSmooth > 0) transform.localRotation = Quaternion.Euler(closedRotation);
            }
        }

        private void Update()
        {
            if (isMoving)
            {
                if (isOpen) Move(openPosition);
                else if (!isOpen) Move(closedPosition);
            }

            if (isRotating)
            {
                if (isOpen) Rotate(openRotation);
                else if (!isOpen) Rotate(closedRotation);
            }
        }

        public override bool CanInteract() => true;

        public override void Interact()
        {
            isOpen = !isOpen; 
            StartRotate();
            StartMove();
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

        void StartMove()
        {
            moveI = 0;
            isMoving = true;
        }

        void EndMove()
        {
            moveI = 0;
            isMoving = false;
        }

        float rotateI = 0, moveI = 0;

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

        void Move(Vector3 position)
        {
            moveI += Time.deltaTime * positionSmooth;

            //apply target rotation
            transform.localPosition = Vector3.Lerp(transform.localPosition, position, moveI);

            if (moveI >= 1)
            {
                EndMove();
            }
        }
    }
}