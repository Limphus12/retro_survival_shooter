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
        [SerializeField] private float walkSpeed = 3.0f;
        [SerializeField] private float runSpeed = 6.0f, crouchSpeed = 1.0f, crouchRunSpeed = 2.0f, jumpSpeed = 5.0f, gravity = 20.0f, antiBumpAmount = 40.0f;

        [Space]
        [SerializeField] private float speedSmoothRate;

        [Header("Camera Settings")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float lookSpeed = 1.0f, lookXLimit = 90.0f;

        [Space]
        [SerializeField] private bool cameraLean = true;
        [SerializeField] private float cameraLeanAmount = 4.0f;

        [Space]
        [SerializeField] private Transform playerCameraHolder;

        [Header("Stance Settings")]
        [SerializeField] private float standingHeight = 2.0f;
        [SerializeField] private float crouchingHeight = 1.0f;

        [Space]
        [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
        [SerializeField] private Vector3 crouchingCenter = new Vector3(0, -0.5f, 0), standingCameraPosition = new Vector3(0, 0.5f, 0), crouchingCameraPosition = new Vector3(0, 0, 0);

        [Space]
        [SerializeField] private float cameraSmoothRate = 5f;

        [Header("Input Settings")]
        [SerializeField] private KeyCode runKey = KeyCode.LeftShift;
        [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

        [HideInInspector]
        public bool canMove = true;

        private CharacterController characterController;
        private Vector3 moveDirection = Vector3.zero;
        private float rotationX = 0, originalStepOffset, currentSpeed, currentHalfHeight;
        private bool isCrouching, isRunning, isJumping, hitCeiling, isCoyoteTime, initRecoveringStamina, recoveringStamina;

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
            Inputs();
        }

        void Inputs()
        {
            //if we're pressing the crouch key and not the sprint key
            if (Input.GetKey(crouchKey) && !Input.GetKey(runKey))
            {
                Crouch();
            }

            //else if we're only pressing the run key and we have the space to stand (just incase we go from crouching to running)
            //btw not sure if we actually need to do the check? idk.
            else if (Input.GetKey(runKey) && !HitCeiling(currentHalfHeight)) //add a stamina check later on
            {
                Run();
            }

            //else if were not pressing the crouch or run key, and we have the room to stand
            else if (!HitCeiling(currentHalfHeight)) Stand();

            CalculateMovement();
        }

        //Calculates Player Movement
        void CalculateMovement()
        {
            //Recalculate move direction based on axes, as we are grounded.
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            //calculate current speed in the Horizontal and Vertical directions (WASD)
            float curSpeedX = canMove ? currentSpeed * Input.GetAxis("Horizontal") : 0;
            float curSpeedZ = canMove ? currentSpeed * Input.GetAxis("Vertical") : 0;

            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * curSpeedZ) + (right * curSpeedX);

            //isGrounded is checked first to ensure the y velocity is reset when the player
            //hits the ground. After that, check for the player input, and jump if so.
            if (characterController.isGrounded)
            {
                characterController.stepOffset = originalStepOffset;

                //if we hit the ground
                if (HitGround())
                {
                    //fixes a stutter issue when we're going down small slopes
                    moveDirection.y = -antiBumpAmount;
                }
                
                //jumping!
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

            //if we hit the ceiling when jumping, cancel our vertical velocity.
            if (!HitCeiling(currentHalfHeight))
            {
                moveDirection.y = 0;
            }

            Move();
        }

        //float zLean;

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

                if (!cameraLean)
                {
                    playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                }

                //camera lean, sets the camera's Z rotation based off horizontal input
                else if (cameraLean)
                {
                    float currentX = Input.GetAxis("Horizontal");
                    playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, currentX * -cameraLeanAmount);

                    #region testing
                    //if (Input.GetAxis("Mouse X") > 0.1f)
                    {
                        //zLean = Mathf.Lerp(zLean, -cameraLeanAmount, Time.deltaTime * 5f);
                    }

                    //else if (Input.GetAxis("Mouse X") < -0.1f)
                    {
                        //zLean = Mathf.Lerp(zLean, cameraLeanAmount, Time.deltaTime * 5f);
                    }

                    //else if (Input.GetAxis("Mouse X") < 0.1f && Input.GetAxis("Mouse X") > -0.1f)
                    {
                        //zLean = Mathf.Lerp(zLean, 0, Time.deltaTime * 2.5f);
                    }

                    //zLean = Mathf.Clamp(zLean, -1f, 1f);

                    //playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, zLean);
                    //transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
                    #endregion
                }

                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            }
        }

        #region Stance Methods

        void Crouch()
        {
            isCrouching = true;
            isRunning = false;

            ChangeStance(crouchingHeight, crouchingCenter, crouchingCameraPosition);
            ChangeSpeed(crouchSpeed);
        }

        void Stand()
        {
            isCrouching = false;

            ChangeStance(standingHeight, standingCenter, standingCameraPosition);
            ChangeSpeed(walkSpeed);
        }

        void Run()
        {
            isRunning = true;

            ChangeStance(standingHeight, standingCenter, standingCameraPosition);

            //if we're not crouching, use our run speed
            if (!isCrouching)
            {
                ChangeSpeed(runSpeed);
            }

            //if we're crouching, use our crouch run speed (like project zomboid lmao).
            else if (isCrouching)
            {
                ChangeSpeed(crouchRunSpeed);
            }
        }

        private Vector3 previousCameraPos;

        private float stanceI = 0f;

        void ChangeStance(float height, Vector3 center, Vector3 cameraPos)
        {
            //If our previous pos is not our current inputed pos, reset stanceI to 0
            if (previousCameraPos != cameraPos)
            {
                previousCameraPos = cameraPos;
                stanceI = 0f;
            }

            characterController.height = height;
            characterController.center = center;

            playerCameraHolder.localPosition = Vector3.Lerp(playerCameraHolder.localPosition, cameraPos, (stanceI + Time.deltaTime) * cameraSmoothRate);

            //calculate current half height (used in determining if we can either stand or for when we hit the ceiling whilst jumping).
            currentHalfHeight = height / 2;
        }

        #endregion

        #region HitChecks

        bool HitCeiling(float currentHalfHeight)
        {
            //raycast upwards from our center
            if (Physics.Raycast(transform.position, transform.up, currentHalfHeight + 0.1f))
            {
                return true;
            }

            else return false;
        }

        bool HitGround()
        {
            //raycast downwards from our center
            if (Physics.Raycast(transform.position, Vector3.down, currentHalfHeight + 0.25f))
            {
                return true;
            }

            else return false;
        }

        #endregion

        #region Speed Methods

        private float previousSpeed, speedI = 0f;

        void ChangeSpeed(float speed)
        {
            //if our previous speed is not our current inputed speed, reset speedI to 0
            if (previousSpeed != speed)
            {
                previousSpeed = speed;
                speedI = 0f;
            }

            currentSpeed = Mathf.Lerp(currentSpeed, speed, (speedI + Time.deltaTime) * speedSmoothRate);
        }

        #endregion
    }
}