using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] private GameObject currentHandItem;
        [SerializeField] private List<GameObject> inventoryItems = new List<GameObject>();

        [Space]
        [SerializeField] private GameObject playerHand;
        [SerializeField] private GameObject playerInventory;
        [SerializeField] private Transform playerCamera;

        [Space]
        [SerializeField] private float interactionDistance = 5f;
        [SerializeField] private float dropForce = 5f;

        private int selectedItem;

        private bool inInventory, enterInput, pickupInput, dropInput;
        private float scrollInput;

        // Start is called before the first frame update
        void Start()
        {
            //grabs the currently equipped item (if we have any)
            if (playerHand.transform.childCount > 0)
            {
                SetHandItem(playerHand.transform.GetChild(0).gameObject);
            }

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

            if (!playerCamera) playerCamera = Camera.main.transform;
        }

        // Update is called once per frame
        void Update() => Inputs();

        void Inputs()
        {
            //flips between in/out of inventory
            if (Input.GetKeyDown(KeyCode.B)) inInventory = !inInventory;

            scrollInput = Input.GetAxis("Mouse ScrollWheel");

            if (Input.GetKeyDown(KeyCode.E)) pickupInput = true;
            else pickupInput = false;

            if (Input.GetKeyDown(KeyCode.Q)) dropInput = true;
            else dropInput = false;

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
                
                ShowInventoryItems();
            }

            //if we're not in the inventory
            else if (!inInventory)
            {
                //check the interact input, to see if we are attempting to either;
                //pick up an item
                if (pickupInput)
                {
                    //do a raycast here
                    RaycastHit hit;

                    if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, interactionDistance))
                    {
                        Item item = hit.transform.GetComponent<Item>();

                        if (item && hit.transform.gameObject != currentHandItem)
                        {
                            //if we dont have a hand item,
                            //put the item in our hand
                            if (GetHandItem() == null)
                            {
                                SetHandItem(hit.transform.gameObject);

                                hit.transform.parent = playerHand.transform;

                                ToggleItem(currentHandItem, true);

                                Debug.Log("Added item to Hand");

                                return;
                            }

                            //if we have a hand item,
                            //send the item to our inventory
                            else if (GetHandItem() != null)
                            {
                                ToggleItem(hit.transform.gameObject, false);

                                AddInventoryItem(hit.transform.gameObject);

                                hit.transform.parent = playerInventory.transform;

                                Debug.Log("Added item to Inventory");

                                return;
                            }
                        }
                    }
                }

                //or drop our current hand item
                else if (dropInput)
                {
                    //if we have a hand item
                    if (GetHandItem() != null)
                    {
                        currentHandItem.transform.parent = null;

                        ToggleItem(currentHandItem, false);

                        RemoveHandItem();

                        Debug.Log("Dropeed our item from Hand");
                    }
                }

                //finally, hide the inventory items
                HideInventoryItems();
            }
        }

        void ShowInventoryItems()
        {
            //for (int i = 0; i < inventoryItems.Count; i++)
            {
                //if (i == selectedItem) inventoryItems[i].SetActive(true);

                //else inventoryItems[i].SetActive(false);
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

                //activate or deactivate the items
                currentHandItem.SetActive(false); inventoryItems[selectedItem].SetActive(true);

                //add or remove them from the inventory list
                AddInventoryItem(toInventoryItem); RemoveInventoryItem(fromInventoryItem);

                //instantly move them to their new positions
                MoveItem(toInventoryItem, playerInventory.transform.position); MoveItem(fromInventoryItem, playerHand.transform.position);

                //sets our hand item
                SetHandItem(fromInventoryItem);

                //finally, we toggle them off or on
                ToggleItem(toInventoryItem, false); ToggleItem(fromInventoryItem, true);
            }

            //grabs the current selected inventory item, and puts it in our hand.
            else if (inventoryItems[selectedItem] != null && currentHandItem == null)
            {
                GameObject fromInventoryItem = inventoryItems[selectedItem];

                //set the parent
                fromInventoryItem.transform.parent = playerHand.transform;

                //activate the item
                inventoryItems[selectedItem].SetActive(true);

                //remove the item from the inventory list
                RemoveInventoryItem(fromInventoryItem);

                //instantly move the item to its new position
                MoveItem(fromInventoryItem, playerHand.transform.position);

                //sets our hand item
                SetHandItem(fromInventoryItem);

                //finally, we toggle the item on
                ToggleItem(fromInventoryItem, true);
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

        public GameObject GetHandItem()
        {
            if (currentHandItem != null) return currentHandItem;

            else return null;
        }

        public void SetHandItem(GameObject item)
        {
            currentHandItem = item;

            //grab the item component and toggle the isEquipped bool
            currentHandItem.GetComponent<Item>().ToggleEquip(true);
        }

        public void RemoveHandItem()
        {
            if (currentHandItem) currentHandItem = null;
        }

        //a method to move an item
        public void MoveItem(GameObject item, Vector3 position)
        {
            item.transform.position = position;
        }

        void ToggleItem(GameObject item, bool b)
        {
            //try to grab all the components and disable them!
            Item itemScript = item.GetComponent<Item>(); if (itemScript) itemScript.enabled = b;

            ItemSway sway = item.GetComponent<ItemSway>(); if (sway) sway.enabled = b;

            //attempt to grab the rigibody, collider and animator
            //so we can toggle the kinematics and collider, and toggle anims

            Rigidbody rb = item.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = b;

                if (!b) rb.AddForce(playerCamera.forward * dropForce, ForceMode.Impulse);
            }

            Collider cl = item.GetComponent<Collider>(); if (cl) { cl.isTrigger = b; cl.enabled = !b; }

            Animator anim = item.GetComponent<Animator>(); if (anim) anim.enabled = b;

            //TODO: add layer functions when we get the proper fps rendering implemented.
        }
    }
}