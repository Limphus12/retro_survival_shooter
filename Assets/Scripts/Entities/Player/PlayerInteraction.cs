using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Attributes - Interaction")]
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        [SerializeField] private float interactionDistance = 2.0f;

        private PlayerController playerController;
        private PlayerInventory playerInventory;
        private PlayerStats playerStats;

        private Camera playerCamera;

        private bool isInteracting;

        public IInteractable Interactable { get; private set; }

        private InteractableItem interactableItem;

        public bool CanInteract { get; private set; }

        public class OnInteractEventArgs : EventArgs { public InteractableItem i; }

        public event EventHandler<OnInteractEventArgs> OnInteract;

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
        }

        private void Update() => Inputs();

        void Inputs()
        {
            //check if we're attempting to interact
            isInteracting = Input.GetKeyDown(interactKey);

            FindInteract();

            if (Interactable != null) CheckInteract();
        }

        private void FindInteract()
        {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactionDistance))
            {
                //attempt to grab the interactable conmponent
                Interactable = hit.transform.GetComponent<IInteractable>();

                interactableItem = hit.transform.GetComponent<InteractableItem>();
            }

            else ResetInteract();
        }

        private void CheckInteract()
        {
            if (!Interactable.CanInteract()) CanInteract = false;

            else if (Interactable.CanInteract())
            {
                CanInteract = true;

                if (isInteracting) Interact();
            }
        }

        private void Interact() { Interactable.Interact(); OnInteract?.Invoke(this, new OnInteractEventArgs { i = interactableItem }); ResetInteract(); }

        private void ResetInteract() { Interactable = null; interactableItem = null; }
    }
}