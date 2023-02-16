using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public enum InventorySlotType { WEAPON, TOOL, CONSUMABLE }

    public class PlayerInventorySlot : MonoBehaviour
    {
        [Header("Attributes - Inventory Slot")]
        [SerializeField] private InventorySlotType slotType;

        [Space]
        [SerializeField] private GameObject slotItem;

        private void Start()
        {
            //grab the child for our slot item
            if (gameObject.transform.childCount > 0) slotItem = gameObject.transform.GetChild(0).gameObject;
        }



        public GameObject GetSlotItem()
        {
            if (slotItem != null) return slotItem;

            else return null;
        }

        public void SetSlotItem(GameObject item)
        {
            //if we already have a slot item
            if (slotItem != null)
            {
                RemoveSlotItem(); //remove the previous slot item
            }

            slotItem = item;

            //grab the item component and toggle the isEquipped bool
            slotItem.GetComponent<Item>().ToggleEquip(true);
        }

        public void RemoveSlotItem()
        {
            if (slotItem) slotItem = null;
        }
    }
}