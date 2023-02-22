using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace com.limphus.retro_survival_shooter
{
    public enum PlayerMovementState { WALKING, RUNNING, CROUCHING }

    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 3.0f;
        [SerializeField] private float runSpeed = 6.0f, crouchSpeed = 1.0f, jumpSpeed = 5.0f, gravity = 20.0f, antiBumpAmount = 40.0f;

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

        [Header("Interaction Settings")]
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        [SerializeField] private float interactionDistance = 2.0f;

        [Space]
        [SerializeField] private GameObject interactionUI;
        [SerializeField] private GameObject interactingUI;

        private GameObject currentInteractableObject;

        [Header("Debug Settings")]
        [SerializeField] private bool debug;

        private bool canMove = true, canRotate = true;

        public void ToggleCanMove(bool b) => canMove = b;
        public void ToggleCanRotate(bool b) => canRotate = b;

        private CharacterController characterController;
        private Vector3 moveDirection = Vector3.zero;
        private float rotationX = 0, originalStepOffset, currentSpeed, currentCeilingRaycast, currentGroundRaycast;
        private bool isCrouching, isRunning, isJumping, isCoyoteTime;
        private bool isInteracting;

        private IInteractable interactable;

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
            Inputs();
            Interactions();
        }

        void Inputs()
        {
            //check if we're attempting to interact
            isInteracting = Input.GetKey(interactKey); 

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
            curSpeedX = currentSpeed * Input.GetAxis("Horizontal");
            curSpeedZ = currentSpeed * Input.GetAxis("Vertical");

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
            if (canMove) characterController.Move(moveDirection * Time.deltaTime);

            //Player and Camera rotation
            if (canRotate)
            {
                rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

                if (!cameraLean)
                {
                    playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                }

                //camera lean, sets the camera's Z rotation based off horizontal input
                else if (cameraLean && canMove)
                {
                    float currentX = Input.GetAxis("LeanHorizontal");
                    playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, currentX * -cameraLeanAmount);
                }

                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            }
        }

        #endregion

        #region Stance Methods

        public PlayerMovementState GetMovementState()
        {
            if (isRunning) return PlayerMovementState.RUNNING;

            else if (isCrouching) return PlayerMovementState.CROUCHING;

            else return PlayerMovementState.WALKING;
        }

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
            //EDIT: No, we just gonna un-crouch and run
            else if (isCrouching)
            {
                isCrouching = false;

                ChangeStance(standingHeight, standingCenter, standingCameraPosition);
                ChangeSpeed(runSpeed);
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
                currentCeilingRaycast = crouchingHeight / 2 + 0.1f;
                currentGroundRaycast = crouchingHeight / 2 + 0.25f;
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

        void Interactions()
        {
            //if we have no interactable
            if (interactable == null && currentInteractableObject == null)
            {
                //do a raycast to find interactable objects
                RaycastHit hit;
                if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance))
                {
                    IInteractable interactable = hit.transform.GetComponent<IInteractable>();

                    //if we manage to grab the interactable
                    if (interactable != null)
                    {
                        interactionUI.SetActive(true);

                        //if we're holding down the interact key - 'E'
                        if (isInteracting)
                        {
                            //call the interact method and set our current interactable 
                            interactable.StartInteract(); this.interactable = interactable;

                            //and interactable obj
                            currentInteractableObject = hit.transform.gameObject;
                        }
                    }

                    else interactionUI.SetActive(false);
                }

                else interactionUI.SetActive(false);
            }

            //if we have an interactable and we're holding down the interact key - 'E'
            else if (interactable != null && currentInteractableObject != null && isInteracting)
            {
                interactionUI.SetActive(false);
                interactingUI.SetActive(true);

                //attempt to grab the container script from our interactable
                Container container = currentInteractableObject.GetComponent<Container>();

                if (container)
                {
                    //make sure we cannot move whilst interacting with the container
                    canMove = false;

                    //attempt to cast this container to the different types

                    //ammo
                    AmmoContainer ammo = container as AmmoContainer;

                    //if we grab the reference
                    if (ammo)
                    {
                        //if we're not looting
                        if (!ammo.IsLooting())
                        {
                            //then start looting!
                            ammo.StartLoot();
                        }
                    }

                    //consumable
                    ConsumableContainer consumable = container as ConsumableContainer;

                    //if we grab the reference
                    if (consumable)
                    {
                        //if we're not looting
                        if (!consumable.IsLooting())
                        {
                            consumable.StartLoot();
                        }
                    }
                }
            }

            //if we have an interactable and we're not holding down the interact key - 'E'
            else if (interactable != null && currentInteractableObject != null && !isInteracting)
            {
                interactingUI.SetActive(false);

                //stop interacting with the interactable
                interactable.StopInteract();

                //make sure we can move again
                canMove = true;

                //attempt to grab the container script from our interactable
                Container container = currentInteractableObject.GetComponent<Container>();

                if (container)
                {
                    //attempt to cast this container to the different types

                    //ammo
                    //AmmoContainer ammo = (AmmoContainer)container; //old cast, threw invalid cast 
                    AmmoContainer ammo = container as AmmoContainer; //new cast, works!

                    //if we grab the reference
                    if (ammo)
                    {
                        //if we're looting
                        if (ammo.IsLooting())
                        {
                            //stop looting!
                            ammo.StopLoot();
                        }
                    }

                    //consumable
                    ConsumableContainer consumable = container as ConsumableContainer;

                    //if we grab the reference
                    if (consumable)
                    {
                        //if we're looting
                        if (consumable.IsLooting())
                        {
                            consumable.StopLoot();
                        }
                    }
                }

                //set our interactable to null
                interactable = null;
                currentInteractableObject = null;
            }

            else
            {
                //make sure we can move again
                canMove = true;

                //set our UI to inactive
                interactionUI.SetActive(false);
                interactingUI.SetActive(false);
            }
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