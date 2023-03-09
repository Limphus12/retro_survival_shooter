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

        public GameObject GetCurrentHandItem() => handItem;

        public void SetPickupItem(GameObject obj)
        {
            //attempt a grab of the new firearm script
            NewFirearm newFirearm = obj.GetComponent<NewFirearm>();

            if (newFirearm && handItem != null)
            {
                NewFirearm newFirearm1 = handItem.GetComponent<NewFirearm>();

                if (newFirearm1 && newFirearm.GetFirearmType() == newFirearm1.GetFirearmType())
                {
                    newFirearm1.SetAmmoCount(newFirearm.GetAmmoCount() + newFirearm1.GetAmmoCount());

                    Destroy(obj); return;
                }
            }

            pickUpItem = obj;
        }

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

            if (handItem != null)
            {
                return;

                NewFirearm newFirearm = handItem.GetComponent<NewFirearm>();

                if (newFirearm)
                {
                    newFirearm.ToggleEquip(false);
                    Invoke(nameof(SwapItem), newFirearm.GetDeEquipTime());
                }

                else SwapItem();
            }

            else if (handItem == null)
            {
                NewFirearm newFirearm = pickUpItem.GetComponent<NewFirearm>();

                if (newFirearm) newFirearm.ToggleEquip(true);

                PickUpItem();
            }
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

            //oh and check if we need to toggle the equip
            NewFirearm newFirearm = pickUpItem.GetComponent<NewFirearm>();

            if (newFirearm) newFirearm.ToggleEquip(true);
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
            if (handItem != null)
            {
                NewFirearm newFirearm = handItem.GetComponent<NewFirearm>();

                if (newFirearm)
                {
                    newFirearm.ToggleEquip(false);
                    Invoke(nameof(DropItem), newFirearm.GetDeEquipTime());
                }

                else DropItem();
            }

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

                UseableItem useableItem = item.GetComponent<UseableItem>();

                if (useableItem) useableItem.PlaceOnFloor();
            }

            else if (b) //if we equipping
            {
                UseableItem useableItem = item.GetComponent<UseableItem>();

                if (useableItem) useableItem.PlaceInHand();
            }

            //TODO: add layer functions when we get the proper fps rendering implemented.
        }
    }
}