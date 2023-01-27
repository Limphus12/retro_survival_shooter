using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private List<GameObject> items;

        private int selectedItem;

        private bool backpackInput;
        private float scrollInput;

        // Start is called before the first frame update
        void Start()
        {
            items = new List<GameObject>();

            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject item = transform.GetChild(i).GetComponent<GameObject>();

                if (item != null)
                {
                    items.Add(item);
                }
            }
        }

        // Update is called once per frame
        void Update() => Inputs();

        void Inputs()
        {
            if (Input.GetKeyDown(KeyCode.B)) backpackInput = true;
            else if (Input.GetKeyUp(KeyCode.B)) backpackInput = false;

            scrollInput = Input.GetAxis("Mouse ScrollWheel");

            CheckInventory();
        }

        void CheckInventory()
        {
            int previousSelectedItem = selectedItem;

            if (scrollInput > 0)
            {
                if (selectedItem >= items.Count - 1) selectedItem = 0;

                else selectedItem++;
            }

            else if (scrollInput < 0)
            {
                if (selectedItem <= 0) selectedItem = items.Count - 1;

                else selectedItem--;
            }

            if (previousSelectedItem != selectedItem) SelectItem();
        }

        void SelectItem()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (i == selectedItem) items[i].gameObject.SetActive(true);

                else items[i].gameObject.SetActive(false);
            }
        }

        //method to get items from this class
        public List<GameObject> GetItems()
        {
            if (items.Count > 0) return items;

            else return null;
        }

        //method to set items in this class
        public void SetItems(List<GameObject> items)
        {
            this.items = items;
        }

        //a method to add an item
        public void AddItem(GameObject item)
        {
            items.Add(item);
        }

        //a method to remove an item
        public void RemoveItem(GameObject item)
        {
            items.Remove(item);
        }
    }
}