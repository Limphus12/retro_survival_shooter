using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Attributes - Inventory")]
        [SerializeField] private PlayerInventorySlot[] playerInventorySlots;
        [SerializeField] private KeyCode[] slotKeys;

        [Space]
        private Transform playerCamera;
        [SerializeField] private int selectedItem, previousSelectedItem;
        private bool inInventory, enterInput, pickupInput, dropInput;
        private float scrollInput;

        private GameObject currentItem;

        private void SetCurrentItem(GameObject item) => currentItem = item;
        public GameObject GetCurrentItem() => currentItem;

        // Start is called before the first frame update
        void Start()
        {
            if (!playerCamera) playerCamera = Camera.main.transform;

            SelectItem();

            //TODO: Assign slots based off the children of a slot parent
        }

        // Update is called once per frame
        void Update() => CheckInput();

        void CheckInput()
        {
            previousSelectedItem = selectedItem;

            //if we have enough keys & inventory slots
            if (slotKeys.Length == playerInventorySlots.Length)
            {
                //run through our slot keys, and see if we press any of them down
                for (int i = 0; i < slotKeys.Length; i++)
                {
                    //if we do, set our selected item
                    //call the select item method and return;
                    if (Input.GetKeyDown(slotKeys[i]))
                    {
                        selectedItem = i;
                    }
                }

                scrollInput = Input.GetAxis("Mouse ScrollWheel");

                if (scrollInput < 0)
                {
                    if (selectedItem >= playerInventorySlots.Length - 1) selectedItem = 0;

                    else selectedItem++;
                }

                else if (scrollInput > 0)
                {
                    if (selectedItem <= 0) selectedItem = playerInventorySlots.Length - 1;

                    else selectedItem--;
                }

                if (selectedItem != previousSelectedItem) SelectItem();
            }

            else Debug.LogWarning("We do not have enough Keys for our Slots/Slots for our Keys!");
        }

        void SelectItem()
        {
            //make sure to deselect the previous item
            ToggleItem(playerInventorySlots[previousSelectedItem].GetSlotItem(), false);

            //then select the new item
            ToggleItem(playerInventorySlots[selectedItem].GetSlotItem(), true);

            //and set this item as our current item
            SetCurrentItem(playerInventorySlots[selectedItem].GetSlotItem());
        }

        void ToggleItem(GameObject item, bool b)
        {
            if (item == null) return;

            //try to grab all the components on the item and toggle them!
            Item itemScript = item.GetComponent<Item>(); if (itemScript)
            {
                itemScript.ToggleEquip(b);
                itemScript.enabled = b;
            }

            ItemSway sway = item.GetComponent<ItemSway>(); if (sway) sway.enabled = b;
            Animator anim = item.GetComponent<Animator>(); if (anim) anim.enabled = b;

            //finally, toggle the item itself!
            item.SetActive(b);

            //TODO: move the item to somewhere else on the player
            //so they can see i.e. a pistol on their waist

            //TODO: add layer functions when we get the proper fps rendering implemented.
        }

        public bool CanAddItem(ItemType itemType)
        {
            foreach(PlayerInventorySlot slot in playerInventorySlots)
            {
                GameObject item = slot.GetSlotItem();

                //if we have no item on the player inventory slot
                if (!item)
                {
                    //check if the slot can take the same item type
                    if (slot.GetItemType() == itemType)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void AddItem(GameObject item, ItemType itemType)
        {
            if (itemType == ItemType.AMMO)
            {
                Debug.Log("Attempting to add ammo to inventory!");
                return;
            }

            foreach (PlayerInventorySlot slot in playerInventorySlots)
            {
                GameObject slotItem = slot.GetSlotItem();

                //if we have no item on the player inventory slot
                if (!slotItem)
                {
                    //check if the slot can take the same item type
                    if (slot.GetItemType() == itemType)
                    {
                        slot.SetSlotItem(item); return;
                    }
                }
            }
        }

        public List <Firearm> GetFirearms()
        {
            List<Firearm> firearms = new List<Firearm>();

            foreach (PlayerInventorySlot slot in playerInventorySlots)
            {
                if (slot.GetItemType() == ItemType.WEAPON)
                {
                    GameObject slotItem = slot.GetSlotItem();

                    if (slotItem)
                    {
                        Firearm firearm = slotItem.GetComponent<Firearm>();

                        if (firearm) firearms.Add(firearm);
                    }
                }
            }

            if (firearms.Count > 0) return firearms;

            else return null;
        }

        public List<Consumable> GetConsumables()
        {
            List<Consumable> consumables = new List<Consumable>();

            foreach (PlayerInventorySlot slot in playerInventorySlots)
            {
                GameObject slotItem = slot.GetSlotItem();

                if (slotItem)
                {
                    Consumable consumable = slotItem.GetComponent<Consumable>();

                    if (consumable) consumables.Add(consumable);
                }
            }

            if (consumables.Count > 0) return consumables;

            else return null;
        }
    }
}