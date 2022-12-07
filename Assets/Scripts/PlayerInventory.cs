using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private List<Item> items;

        // Start is called before the first frame update
        void Start()
        {
            //items = new List<Item>();

            for (int i = 0; i < transform.childCount; i++)
            {
                Item item = transform.GetChild(i).GetComponent<Item>();

                if (item != null)
                {
                    //items.Add(item);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        //method to get items from this class
        public List<Item> GetItems()
        {
            if (items.Count > 0) return items;

            else return null;
        }

        //method to set items in this class
        public void SetItems(List<Item> items)
        {
            this.items = items;
        }
    }
}