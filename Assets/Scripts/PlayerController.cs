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


        [Header("Debug Settings")]
        public bool debug;

        private bool canMove = true;

        public void ToggleCanMove(bool b)
        {
            canMove = b;
        }

        private CharacterController characterController;
        private Vector3 moveDirection = Vector3.zero;
        private float rotationX = 0, originalStepOffset, currentSpeed, currentCeilingRaycast, currentGroundRaycast;
        private bool isCrouching, isRunning, isJumping, isCoyoteTime;

        private PlayerStats playerStats;

        //Start is called before the first frame update
        void Start()
        {
            //Grabs the CharacterController from the player object
            if (!characterController) characterController = GetComponent<CharacterController>();

            //Grabs the camera from the player children
            if (!playerCamera) playerCamera = GetComponentInChildren<Camera>();

            //Grabs the player stats from the player object
            if (!playerStats) playerStats = GetComponent<PlayerStats>();

            //Lock Cursor - replace with a player manager later on?
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            //Set the original step offset - used for a bugfix
            originalStepOffset = characterController.stepOffset;
        }

        //Update is called once per frame
        void Update()
        {
            if (canMove) Inputs();
        }

        void Inputs()
        {
            //if were not pressing the crouch key, and we have the room to stand
            if (!Input.GetKey(crouchKey) && !HitCeiling(currentCeilingRaycast))
            {
                Stand();
            }

            //else if we're pressing the crouch key
            else if (Input.GetKey(crouchKey))
            {
                Crouch();
            }

            //if we're not pressing the run key, then walk (sets our speed to either crouch speed or walk speed)
            if (!Input.GetKey(runKey))
            {
                //if we have the playerstats reference, cancel the stamina depletion tick
                if (playerStats)
                {
                    //if we're not invoking the stamina replenish tick, start invoking it
                    if (!playerStats.IsInvoking(nameof(playerStats.StaminaReplenishTick)))
                    {
                        //call the replenish stamina
                        playerStats.ReplenishStamina();

                        //cancel the stamina depletion tick
                        playerStats.CancelStaminaDepletionTick();

                        //and walk!
                        Walk();
                    }

                    //else walk!
                    else Walk();
                }
            }

            //else if we're only pressing the run key and we have the space to stand (just incase we go from crouching to running)
            //btw not sure if we actually need to do the check? idk. EDIT, NO CEILING CHECK, AS WE CAN RUN WHILST CROUCHING
            else if (Input.GetKey(runKey)) //add a stamina check later on (DONE)
            {
                //if we have the playerstats reference and we have the stamina to spare
                if (playerStats && playerStats.GetCurrentStamina() > 0)
                {
                    //if we are moving
                    if (curSpeedX != 0 || curSpeedZ != 0)
                    {
                        //if we're not invoking the stamina depletion tick, start invoking it
                        if (!playerStats.IsInvoking(nameof(playerStats.StaminaDepletionTick)))
                        {
                            //call the deplete stamina
                            playerStats.DepleteStamina();

                            //cancel the stamina replenish tick
                            playerStats.CancelInvoke(nameof(playerStats.StaminaReplenishTick));

                            //and run!
                            Run();
                        }

                        //if we are invoking, then run!
                        else Run();
                    }

                    //if we are not moving
                    else if (curSpeedX == 0 || curSpeedZ == 0)
                    {
                        //if we're invoking the stamina depletion tick, stop invoking it
                        if (playerStats.IsInvoking(nameof(playerStats.StaminaDepletionTick)))
                        {
                            //cancel the stamina depletion tick
                            playerStats.CancelStaminaDepletionTick();

                            //if we're not invoking the stamina replenish tick, start invoking it
                            if (!playerStats.IsInvoking(nameof(playerStats.StaminaReplenishTick)))
                            {
                                //call the replenish stamina
                                playerStats.ReplenishStamina();
                            }

                            //and 'run'!
                            Run();
                        }

                        //if we are invoking, then 'run'!
                        else Run();
                    }
                }
            }

            CalculateMovement();
        }

        #region Movement

        private float curSpeedX, curSpeedZ;

        //Calculates Player Movement
        void CalculateMovement()
        {
            //Recalculate move direction based on axes, as we are grounded.
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            //calculate current speed in the Horizontal and Vertical directions (WASD)
            curSpeedX = canMove ? currentSpeed * Input.GetAxis("Horizontal") : 0;
            curSpeedZ = canMove ? currentSpeed * Input.GetAxis("Vertical") : 0;

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

                //if we hit the ceiling when jumping, cancel our vertical velocity.
                if (HitCeiling(currentCeilingRaycast))
                {
                    moveDirection.y = 0;
                }
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

        #endregion

        #region Stance Methods

        void Crouch()
        {
            isCrouching = true;

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

        void Walk()
        {
            isRunning = false;

            //if we're not crouching, use our walk speed
            if (!isCrouching)
            {
                ChangeSpeed(walkSpeed);
            }

            //if we're crouching, use our crouch speed
            else if (isCrouching)
            {
                ChangeSpeed(crouchSpeed);
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

            //calculate current raycasts used in determining if we can either stand or for when we hit the ceiling whilst jumping
            //as well as the ground check stuff for the antibump
            if (!isCrouching)
            {
                currentCeilingRaycast = standingHeight / 2 + 0.1f;
                currentGroundRaycast = standingHeight / 2 + 0.25f;
            }

            else if (isCrouching)
            {
                currentCeilingRaycast = standingHeight / 2 + 0.1f;
                currentGroundRaycast = standingHeight / 2 + 0.25f;
            }
        }

        #endregion

        #region HitChecks

        bool HitCeiling(float currentRaycastHeight)
        {
            //raycast upwards from our center
            if (Physics.Raycast(transform.position, transform.up, currentRaycastHeight))
            {
                return true;
            }

            else return false;
        }

        bool HitGround()
        {
            //raycast downwards from our center
            if (Physics.Raycast(transform.position, -transform.up, currentGroundRaycast))
            {
                return true;
            }

            else return false;
        }

        #endregion

        #region Speed Methods

        private float previousSpeed, speedI = 0f;

        [Tooltip("Value between 0 and 1; 0 is no speed, 1 is full speed")]
        [Range(0f, 1f)] private float speedPercentage = 1f;

        public float GetSpeedPercentage()
        {
            return speedPercentage;
        }

        public void SetSpeedPercentage(int i) => speedPercentage = i;

        public void ResetSpeedPercentage() => speedPercentage = 1;

        void ChangeSpeed(float speed)
        {
            //if our previous speed is not our current inputed speed, reset speedI to 0
            if (previousSpeed != speed)
            {
                previousSpeed = speed;
                speedI = 0f;
            }

            //calculate our current speed by lerping between the new speed and current speed, and divided by our speed %
            currentSpeed = Mathf.Lerp(currentSpeed, speed, (speedI + Time.deltaTime) * speedSmoothRate) / speedPercentage;
        }

        #endregion

        private void OnDrawGizmos()
        {
            if (debug)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, transform.up * currentCeilingRaycast);

                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, -transform.up * currentGroundRaycast);
            }
        }
    }
}