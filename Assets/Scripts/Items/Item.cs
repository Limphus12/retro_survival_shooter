using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.limphus.retro_survival_shooter
{
    [Serializable]
    public class Item : MonoBehaviour
    {
        [Header("Attributes - Item")]
        [SerializeField] protected ItemData itemData;
        [SerializeField] protected ItemSound itemSound;
        [SerializeField] protected ItemSway itemSway;
        [SerializeField] protected ItemAnimation itemAnimation;

        protected GameObject model;
        protected string itemName;
        protected double itemWeight;

        private void Awake() => Init();

        protected bool isEquipped;

        public bool IsEquipped()
        {
            return isEquipped;
        }

        public virtual void ToggleEquip(bool b)
        {
            isEquipped = b;

            if (isEquipped && itemSound)
            {
                itemSound.PlayEquipSound();
            }
        }

        private void Start()
        {
            //if this weapon is not equipped, then return;
            if (!isEquipped) return;
        }

        protected virtual void Init()
        {
            if (!itemData)
            {
                Debug.LogWarning("No Item Data found for " + gameObject.name + "; Assign Item Data!");
                return;
            }

            model = itemData.prefab;
            itemName = itemData.itemName;
            itemWeight = itemData.itemWeight;

            name = itemName;
        }

        public virtual ItemData GetItemData()
        {
            if (itemData != null) return itemData;

            else return null;
        }

        public virtual void SetItemData(ItemData itemData)
        {
            this.itemData = itemData;

            Init();
        }
    }
}