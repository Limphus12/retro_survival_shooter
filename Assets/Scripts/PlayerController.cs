using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace com.limphus.retro_survival_shooter
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 2.0f;
        [SerializeField] private float runSpeed = 6.0f, jumpSpeed = 6.0f, gravity = 20.0f, antiBumpAmount = 100.0f;

        [Header("Camera Settings")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float lookSpeed = 1.0f, lookXLimit = 90.0f;

        [Space]
        [SerializeField] private bool cameraLean = true;
        [SerializeField] private float cameraLeanAmount = 4.0f;

        [HideInInspector]
        public bool canMove = true;

        private CharacterController characterController;
        private Vector3 moveDirection = Vector3.zero;
        private float rotationX = 0, originalStepOffset;
        private bool hitCeiling;

        private bool IsRunning()
        {
            if (Input.GetKey(KeyCode.LeftShift)) return true;

            else return false;
        }

        //Start is called before the first frame update
        void Start()
        {
            //Grabs the CharacterController from the player object.
            characterController = GetComponent<CharacterController>();

            //Grabs the camera from the player.
            playerCamera = GetComponentInChildren<Camera>();

            //Lock Cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            //Set the original step offset
            originalStepOffset = characterController.stepOffset;
        }

        //Update is called once per frame
        void Update()
        {
            CalculateMovement();
        }

        //Calculates Player Movement
        void CalculateMovement()
        {
            //Recalculate move direction based on axes, as we are grounded.
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            float curSpeedX = canMove ? IsRunning() ? runSpeed * Input.GetAxis("Horizontal") : walkSpeed * Input.GetAxis("Horizontal") : 0;
            float curSpeedZ = canMove ? IsRunning() ? runSpeed * Input.GetAxis("Vertical") : walkSpeed * Input.GetAxis("Vertical") : 0;

            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * curSpeedZ) + (right * curSpeedX);

            //isGrounded is checked first to ensure the y velocity is reset when the player
            //hits the ground. After that, check for the player input, and jump if so.
            if (characterController.isGrounded)
            {
                hitCeiling = false;

                characterController.stepOffset = originalStepOffset;

                //fixes a stutter issue when we're going down small slopes
                moveDirection.y = -antiBumpAmount;

                if (Input.GetButton("Jump") && canMove)
                {
                    moveDirection.y = jumpSpeed;
                }
            }

            else
            {
                moveDirection.y = movementDirectionY;
            }

            //Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
            //when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
            //as an acceleration (ms^-2)
            if (!characterController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;

                characterController.stepOffset = 0f; //Fixes a bug where jumping against something that, if will end up at your step height during the jump, it would suddenly put you back on the ground. 
            }

            if (Physics.Raycast(transform.position, transform.up, 1.1f) && !hitCeiling)
            {
                hitCeiling = true;

                moveDirection.y = 0;
            }

            Move();
        }

        //Deals with Movement
        void Move()
        {
            //Move the controller
            characterController.Move(moveDirection * Time.deltaTime);

            //Player and Camera rotation
            if (canMove)
            {
                rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

                //Camera Lean, sets the camera's Z rotation based off horizontal input.
                if (!cameraLean)
                {
                    playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                    transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
                }

                else
                {
                    float currentX = Input.GetAxis("Horizontal");

                    playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, currentX * -cameraLeanAmount);
                    transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
                }
            }
        }
    }
}