using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class NewPlayerInventory : MonoBehaviour
    {
        [Header("Attributes - Inventory")]
        [SerializeField] private Transform handSlot;
        [SerializeField] private GameObject handItem;

        [Space]
        [SerializeField] private Transform playerCamera;

        [Space]
        [SerializeField] private KeyCode pickUpKey;
        [SerializeField] private KeyCode dropKey;

        [Space]
        [SerializeField] private GameObject pickUpItem;

        public void SetPickupItem(GameObject obj) => pickUpItem = obj;
        public void RemovePickupItem(GameObject obj)
        {
            if (pickUpItem && pickUpItem == obj)
            {
                pickUpItem = null;
            }

            else Debug.Log("cannot remove pickup item, as it is not referenced!");
        }

        private void Start()
        {
            if (!playerCamera) playerCamera = Camera.main.transform;
        }

        // Update is called once per frame
        void Update() => CheckInput();

        void CheckInput()
        {
            if (Input.GetKeyDown(pickUpKey)) CheckPickUp();

            else if (Input.GetKeyDown(dropKey)) CheckDrop();
        }

        private void CheckPickUp()
        {
            if (pickUpItem == null) return;

            if (handItem != null) SwapItem();

            else if (handItem == null) PickUpItem();
        }

        private void SwapItem()
        {
            //toggle our items
            ToggleItem(handItem, false); ToggleItem(pickUpItem, true);

            //dropping our current hand item
            handItem.transform.parent = null;

            //swap around our items, by setting our hand item to our pickup item
            handItem = pickUpItem;

            //set our new item parent
            handItem.transform.parent = handSlot;
        }

        private void PickUpItem()
        {
            //set our hand item
            handItem = pickUpItem;

            //set the hand item parent
            handItem.transform.parent = handSlot.transform;

            //and toggle it!
            ToggleItem(handItem, true);
        }

        private void CheckDrop()
        {
            if (handItem != null) DropItem();

            else if (handItem == null) Debug.Log("Cannot drop item, since we dont have one!");
        }

        private void DropItem()
        {
            //toggle our current item
            ToggleItem(handItem, false);

            //drop our current item
            handItem.transform.parent = null;

            //and set the hand item to null
            handItem = null;
        }

        void ToggleItem(GameObject item, bool b)
        {
            if (item == null) return;

            Collider[] colliders = item.GetComponents<Collider>();

            if (colliders.Length > 0)
            {
                foreach (Collider collider in colliders)
                {
                    collider.enabled = !b;
                }
            }

            //for new stuff
            NewFirearm newFirearm = item.GetComponent<NewFirearm>(); if (newFirearm)
            {
                newFirearm.enabled = b;
            }

            //try to grab all the components on the item and toggle them!
            Item itemScript = item.GetComponent<Item>(); if (itemScript)
            {
                itemScript.ToggleEquip(b);
                itemScript.enabled = b;
            }

            ItemSway sway = item.GetComponent<ItemSway>(); if (sway) sway.enabled = b;
            Animator anim = item.GetComponent<Animator>(); if (anim) anim.enabled = b;

            if (!b) //if we de-equipping
            {
                //grab the item pick up script, and force it to go onto the floor.

                ItemPickUp itemPickUp = item.GetComponent<ItemPickUp>();

                if (itemPickUp) itemPickUp.PlaceOnFloor();
            }

            else if (b) //if we equipping
            {
                ItemPickUp itemPickUp = item.GetComponent<ItemPickUp>();

                if (itemPickUp) itemPickUp.PlaceInHand();
            }

            //TODO: add layer functions when we get the proper fps rendering implemented.
        }
    }
}