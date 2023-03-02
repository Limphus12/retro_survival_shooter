using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public enum InteractionState { NOTLOOTING, LOOTABLE, LOOTING, UNLOOTABLE }

    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Attributes - Interaction")]
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        [SerializeField] private float interactionDistance = 2.0f;

        [Space]
        [SerializeField] private GameObject interactionUI;
        [SerializeField] private GameObject interactingUI, cannotInteractUI;

        private GameObject currentInteractableObject;

        private PlayerController playerController;
        private PlayerInventory playerInventory;
        private PlayerStats playerStats;

        private Camera playerCamera;

        private bool isInteracting;

        private IInteractable interactable;

        List<Firearm> firearms = new List<Firearm>();
        bool canLoot = false;

        Container container; AmmoContainer ammo; ConsumableContainer consumable;

        private InteractionState interactionState;

        private void Start()
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
            isInteracting = Input.GetKey(interactKey);

            CheckInteractables();
            CheckState();
        }

        private void CheckState()
        {
            switch (interactionState)
            {
                case InteractionState.NOTLOOTING:

                    interactionUI.SetActive(false);
                    interactingUI.SetActive(false);
                    cannotInteractUI.SetActive(false);

                    break;
                case InteractionState.LOOTABLE:

                    interactionUI.SetActive(true);
                    interactingUI.SetActive(false);
                    cannotInteractUI.SetActive(false);

                    break;
                case InteractionState.LOOTING:

                    interactionUI.SetActive(false);
                    interactingUI.SetActive(true);
                    cannotInteractUI.SetActive(false);

                    break;
                case InteractionState.UNLOOTABLE:

                    interactionUI.SetActive(false);
                    interactingUI.SetActive(false);
                    cannotInteractUI.SetActive(true);

                    break;

                default:
                    break;
            }
        }

        void CheckInteractables()
        {
            firearms = playerInventory.GetFirearms();

            //if we have no interactable
            if (interactable == null && currentInteractableObject == null) FindInteractables();

            //else if we managed to find an interactable (and we started interacting with it)
            else if (interactable != null && currentInteractableObject != null) UseInteractables();

            //no raycast hits, no interactable references etc.
            else StopLoot(InteractionState.NOTLOOTING);
        }

        private void FindInteractables()
        {
            //do a raycast to find interactable objects
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance))
            {
                IInteractable interactable = hit.transform.GetComponent<IInteractable>();

                //if we manage to grab the interactable
                if (interactable != null)
                {
                    //check if we can even grab anything (if it's a container)

                    //attempt to grab the container script from our hit object
                    container = hit.transform.GetComponent<Container>();

                    if (container)
                    {
                        //ammo
                        ammo = container as AmmoContainer; //new cast, works!

                        if (ammo)
                        {
                            //if we cannot loot this ammo container (either because we cannot loot or we have no weapons)
                            if (!container.CanLoot() || firearms == null)
                            {
                                interactionState = InteractionState.UNLOOTABLE;
                            }

                            //if we do have weapons
                            else if (firearms != null)
                            {
                                //check if they can grab any reserve ammo
                                foreach (Firearm firearm in firearms)
                                {
                                    //if (firearm.GetCurrentAmmoReserves() < firearm.GetMaxAmmoReserves())
                                    {
                                        //canLoot = true;
                                    }
                                }
                            }

                            if (canLoot) //if we can loot
                            {
                                interactionState = InteractionState.LOOTABLE;

                                //if we press the interact key
                                if (isInteracting)
                                {
                                    //set our current interactable and interactable obj
                                    this.interactable = interactable; currentInteractableObject = hit.transform.gameObject;

                                    //call the interact method
                                    interactable.StartInteract();
                                }
                            }

                            else if (!canLoot)
                            {
                                interactionState = InteractionState.UNLOOTABLE;
                            }
                        }

                        //consumable
                        consumable = container as ConsumableContainer;

                        if (consumable)
                        {
                            //if we cannot
                            if (!container.CanLoot() || !playerInventory.CanAddItem(container.GetItemType()))
                            {
                                interactionState = InteractionState.UNLOOTABLE;
                            }

                            else //if we can
                            {
                                interactionState = InteractionState.LOOTABLE;

                                //if we press the interact key
                                if (isInteracting)
                                {
                                    //set our current interactable and interactable obj
                                    this.interactable = interactable; currentInteractableObject = hit.transform.gameObject;

                                    //call the interact method
                                    interactable.StartInteract();
                                }
                            }
                        }
                    }
                }

                else interactionState = InteractionState.NOTLOOTING;
            }

            else interactionState = InteractionState.NOTLOOTING;
        }

        private void UseInteractables()
        {
            //if we have a container
            if (container)
            {
                if (ammo)
                {
                    //if we cannot loot this ammo container (either because we cannot loot or we have no weapons)
                    if (!container.CanLoot() || firearms == null)
                    {
                        StopLoot(InteractionState.UNLOOTABLE);
                        return;
                    }

                    //if we do have weapons
                    else if (firearms != null)
                    {
                        //check if they can grab any reserve ammo
                        foreach (Firearm firearm in firearms)
                        {
                            //if (firearm.GetCurrentAmmoReserves() < firearm.GetMaxAmmoReserves())
                            {
                                //canLoot = true;
                            }


                            //TODO! CURRENTLY WE CANNOT GET THE AMMO RESERVES!

                        }
                    }

                    //if all of our guns are filled up
                    if (!canLoot)
                    {
                        StopLoot(InteractionState.UNLOOTABLE);
                        return;
                    }
                }

                else if (consumable)
                {
                    //if we cannot loot this consumable container
                    if (!container.CanLoot() || !playerInventory.CanAddItem(container.GetItemType()))
                    {
                        StopLoot(InteractionState.UNLOOTABLE);
                        return;
                    }
                }
            }

            //if we're still holding down the interact key - 'E'
            if (isInteracting)
            {
                interactionState = InteractionState.LOOTING;

                if (container)
                {
                    if (ammo)
                    {
                        //if we cannot loot this ammo container (either because we cannot loot or we have no weapons)
                        if (!container.CanLoot() || firearms == null)
                        {
                            StopLoot(InteractionState.UNLOOTABLE);
                            return;
                        }

                        else if (firearms != null)
                        {
                            //check if they can grab any reserve ammo
                            foreach (Firearm firearm in firearms)
                            {
                                //if (firearm.GetCurrentAmmoReserves() < firearm.GetMaxAmmoReserves())
                                {
                                    //canLoot = true;
                                }


                                //TODO! CURRENTLY WE CANNOT GET THE AMMO RESERVES!

                            }
                        }

                        if (!canLoot)
                        {
                            StopLoot(InteractionState.UNLOOTABLE);
                            return;
                        }

                        else if (!container.IsLooting() && canLoot && container.CanLoot())
                        {
                            CanLoot();
                            return;
                        }
                    }

                    //if we grab the reference
                    if (consumable)
                    {
                        //if we cannot loot the container (either because we cannot loot or we have no spare slots)
                        if (!container.CanLoot() || !playerInventory.CanAddItem(container.GetItemType()))
                        {
                            StopLoot(InteractionState.UNLOOTABLE);
                        }

                        //if we're not looting and we can loot
                        else if (container.CanLoot() && !container.IsLooting())
                        {
                            CanLoot();
                        }
                    }
                }
            }

            //if we've stopped holding down the interact key - 'E'
            else if (!isInteracting)
            {
                StopLoot(InteractionState.NOTLOOTING);
            }
        }

        private void CanLoot()
        {
            interactionState = InteractionState.LOOTING;

            //make sure we cannot move whilst interacting with the container
            playerController.ToggleCanMove(false);

            container.StartLoot();
        }

        private void StopLoot(InteractionState state)
        {
            //stop looting!
            interactionState = state;

            //make sure we can move again
            playerController.ToggleCanMove(true);

            //if we had an interactable
            if (interactable != null && currentInteractableObject != null)
            {
                //stop interacting with the interactable
                interactable.StopInteract();

                if (container.IsLooting()) container.StopLoot();

                //set our interactable to null
                interactable = null;
                currentInteractableObject = null;
            }
        }
    }
}