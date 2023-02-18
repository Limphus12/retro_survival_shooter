using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.limphus.retro_survival_shooter
{
    public enum ItemType { WEAPON, TOOL, CONSUMABLE }

    public class Item : MonoBehaviour
    {
        [Header("Attributes - Item")]
        [SerializeField] private ItemType ItemType;

        [Space]
        [SerializeField] protected ItemData itemData;
        [SerializeField] protected ItemSound itemSound;
        [SerializeField] protected ItemSway itemSway;
        [SerializeField] protected ItemAnimation itemAnimation;

        protected GameObject model;
        protected string itemName;
        protected double itemWeight;

        [Header("Attributes - Item Functions")]
        [SerializeField] private Melee melee;
        [SerializeField] private Firearm firearm;

        [Space]
        [SerializeField] private Throwable throwable;
        [SerializeField] private Placeable placeable;

        [Space]
        [SerializeField] private Consumable consumable;

        private Transform playerCamera;
        private PlayerStats playerStats;

        private void Awake() => Init();

        protected bool isEquipped;

        private bool leftMouseInput, rightMouseInput, reloadInput, meleeInput, previousMeleeInput;

        public bool IsEquipped()
        {
            return isEquipped;
        }

        public void ToggleEquip(bool b)
        {
            isEquipped = b;

            if (isEquipped && itemSound) itemSound.PlayEquipSound();
        }

        private void Start()
        {
            //if this item is not equipped, then return;
            if (!isEquipped) return;
        }

        protected virtual void Init()
        {
            InitStats(); InitReferences();
        }

        protected virtual void InitStats()
        {
            if (!itemData)
            {
                Debug.LogWarning("No Item Data found for " + gameObject.name + "; Assign Item Data!");
                return;
            }

            itemName = itemData.itemName; name = itemName;
            itemWeight = itemData.itemWeight;
        }

        private void InitReferences()
        {
            if (!playerStats) playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
            if (!playerCamera) playerCamera = Camera.main.transform;
        }

        public ItemData GetItemData()
        {
            if (itemData != null) return itemData;

            else return null;
        }

        public void SetItemData(ItemData itemData)
        {
            this.itemData = itemData;

            Init();
        }

        private void Update() => Inputs();

        private void Inputs()
        {
            previousMeleeInput = meleeInput;

            if (Input.GetMouseButtonDown(0)) leftMouseInput = true;
            else if (Input.GetMouseButtonUp(0)) leftMouseInput = false;

            if (Input.GetMouseButtonDown(1)) rightMouseInput = true;
            else if (Input.GetMouseButtonUp(1)) rightMouseInput = false;

            if (Input.GetKeyDown(KeyCode.R)) reloadInput = true;
            else if (Input.GetKeyUp(KeyCode.R)) reloadInput = false;

            if (Input.GetKeyDown(KeyCode.V)) meleeInput = true;
            else if (Input.GetKeyUp(KeyCode.V)) meleeInput = false;

            Functions();
        }

        private void Functions()
        {
            if (melee)
            {
                //if we have no other function types, and we have melee input
                if (!firearm && !throwable && !consumable && !placeable && meleeInput)
                {
                    melee.CheckInputs(meleeInput, previousMeleeInput, rightMouseInput);
                }

                else
                {
                    if (meleeInput) { melee.CheckInputs(meleeInput, previousMeleeInput, rightMouseInput); return; }

                    if (firearm) firearm.CheckInputs(leftMouseInput, rightMouseInput, reloadInput);

                    else if (consumable) consumable.CheckInputs(rightMouseInput);

                    else if (throwable) throwable.CheckInputs();

                    else if (placeable) placeable.CheckInputs();
                }
            }

            else if (!melee)
            {
                if (firearm) firearm.CheckInputs(leftMouseInput, rightMouseInput, reloadInput);

                else if (consumable) consumable.CheckInputs(rightMouseInput);

                else if (throwable) throwable.CheckInputs();

                else if (placeable) placeable.CheckInputs();
            }
        }
    }
}