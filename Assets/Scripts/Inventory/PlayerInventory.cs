using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private GameObject currentHandItem;
        [SerializeField] private List<GameObject> inventoryItems = new List<GameObject>();

        [Space]
        [SerializeField] private GameObject playerHand;
        [SerializeField] private GameObject playerInventory;

        private int selectedItem;

        private bool inInventory, enterInput;
        private float scrollInput;

        // Start is called before the first frame update
        void Start()
        {
            //grabs the currently equipped item
            SetHandItem(playerHand.transform.GetChild(0).gameObject);

            //grabs all the items in the backpack
            for (int i = 0; i < playerInventory.transform.childCount; i++)
            {
                GameObject item = playerInventory.transform.GetChild(i).gameObject;

                if (item != null)
                {
                    AddInventoryItem(item);

                    item.transform.position = Vector3.zero;
                }

                else if (item == null) Debug.Log("didnt find the item!");
            }
        }

        // Update is called once per frame
        void Update() => Inputs();

        void Inputs()
        {
            //flips between in/out of inventory
            if (Input.GetKeyDown(KeyCode.B)) inInventory = !inInventory;

            scrollInput = Input.GetAxis("Mouse ScrollWheel");

            CheckInventory();
        }

        void CheckInventory()
        {
            //if we're in the inventory
            if (inInventory)
            {
                int previousSelectedItem = selectedItem;

                if (scrollInput > 0)
                {
                    if (selectedItem >= inventoryItems.Count - 1) selectedItem = 0;

                    else selectedItem++;
                }

                else if (scrollInput < 0)
                {
                    if (selectedItem <= 0) selectedItem = inventoryItems.Count - 1;

                    else selectedItem--;
                }
                
                ShowInventoryItem();
            }

            //if we're not in the inventory
            else if (!inInventory)
            {
                HideInventoryItems();
            }
        }

        void ShowInventoryItem()
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (i == selectedItem) inventoryItems[i].SetActive(true);

                else inventoryItems[i].SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.Return)) SelectInventoryItem();
        }

        void HideInventoryItems()
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                inventoryItems[i].SetActive(false);
            }
        }

        void SelectInventoryItem()
        {
            //grabs the currently selected inventory item, and swaps it with our current in-hand item.
            if (inventoryItems[selectedItem] != null && currentHandItem != null)
            {
                //store the two items we need to swap
                GameObject toInventoryItem = currentHandItem, fromInventoryItem = inventoryItems[selectedItem];

                //set their parents
                toInventoryItem.transform.parent = playerInventory.transform; fromInventoryItem.transform.parent = playerHand.transform;

                //add or remove them from the inventory list
                AddInventoryItem(toInventoryItem); RemoveInventoryItem(fromInventoryItem);

                //instantly move them to their new positions
                MoveItem(toInventoryItem, playerInventory.transform.position); MoveItem(fromInventoryItem, playerHand.transform.position);

                //sets our hand item
                SetHandItem(fromInventoryItem);

                //finally, we toggle them off or on
                ToggleItem(toInventoryItem, false); ToggleItem(fromInventoryItem, true);
            }
        }

        //method to get items from this class
        public List<GameObject> GetInventoryItems()
        {
            if (inventoryItems.Count > 0) return inventoryItems;

            else return null;
        }

        //method to set items in this class
        public void SetInventoryItems(List<GameObject> items)
        {
            inventoryItems = items;
        }

        //a method to add an item
        public void AddInventoryItem(GameObject item)
        {
            inventoryItems.Add(item);

            //grab the item component and toggle the isEquipped bool
            item.GetComponent<Item>().ToggleEquip(false);
        }

        //a method to remove an item
        public void RemoveInventoryItem(GameObject item)
        {
            inventoryItems.Remove(item);
        }

        public void SetHandItem(GameObject item)
        {
            currentHandItem = item;

            //grab the item component and toggle the isEquipped bool
            currentHandItem.GetComponent<Item>().ToggleEquip(true);
        }

        //a method to move an item
        public void MoveItem(GameObject item, Vector3 position)
        {
            item.transform.position = position;
        }

        void ToggleItem(GameObject item, bool b)
        {
            //try to grab all the components and disable them!
            Item itemScript = item.GetComponent<Item>();

            if (itemScript) itemScript.enabled = b;

            WeaponSway sway = item.GetComponent<WeaponSway>();

            if (sway) sway.enabled = b;
        }
    }
}