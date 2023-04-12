using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Attributes - Interaction")]
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        [SerializeField] private float interactionDistance = 2.0f;

        [Space]
        [SerializeField] private GameObject interactionUI;

        private PlayerController playerController;
        private PlayerInventory playerInventory;
        private PlayerStats playerStats;

        private Camera playerCamera;

        private bool isInteracting, canInteract;

        private IInteractable interactable;

        List<Firearm> firearms = new List<Firearm>();
        bool canLoot = false;

        Container container; AmmoContainer ammo; ConsumableContainer consumable;

        private void Awake() => Init();

        private void Init()
        {
            //Grabs the main camera, as this is the one we have for our player...
            if (!playerCamera) playerCamera = Camera.main;

            //Grabs the CharacterController from the player object
            if (!playerController) playerController = GetComponent<PlayerController>();

            //Grabs the player stats from the player object
            if (!playerStats) playerStats = GetComponent<PlayerStats>();

            //Grabs the player inventory from the player object
            if (!playerInventory) playerInventory = GetComponent<PlayerInventory>();

            if (playerInventory)
            {
                firearms = playerInventory.GetFirearms();
            }
        }

        private void Update() => Inputs();

        void Inputs()
        {
            //check if we're attempting to interact
            isInteracting = Input.GetKey(interactKey);

            CheckInteract();

            interactionUI.SetActive(canInteract);
        }

        private void CheckInteract()
        {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactionDistance))
            {
                //attempt to grab the interactable conmponent
                interactable = hit.transform.GetComponent<IInteractable>();

                //if we do have it
                if (interactable != null)
                {
                    canInteract = false;

                    //check if its a container
                    container = hit.transform.GetComponent<Container>();

                    if (container)
                    {
                        ammo = container.GetComponent<AmmoContainer>();
                        consumable = container.GetComponent<ConsumableContainer>();

                        if (ammo)
                        {

                        }

                        else if (consumable)
                        {
                            if (consumable.CanLoot()) canInteract = true;
                        }
                    }

                    //check if its a door


                    //check if its a 

                    if (isInteracting && canInteract) Interact();
                }
            }

            else ResetInteract();
        }

        private void Interact()
        {
            interactable.Interact();
        }

        private void ResetInteract()
        {
            interactable = null;
            canInteract = false;
        }    
    }
}