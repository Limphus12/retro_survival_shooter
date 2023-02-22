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
        private PlayerController playerController;

        private FirearmAnimation firearmAnimation;
        private MeleeAnimation meleeAnimation;
        private ConsumableAnimation consumableAnimation;

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
            InitStats(); InitReferences(); InitEffects();
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
            if (!playerController) playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            if (!playerCamera) playerCamera = Camera.main.transform;
        }

        private void InitEffects()
        {
            if (itemAnimation)
            {
                if (firearm && !firearmAnimation)
                {
                    //attempt to cast from the item animation
                    firearmAnimation = (FirearmAnimation)itemAnimation;
                }

                if (melee && !meleeAnimation)
                {
                    //attempt to cast from the item animation
                    meleeAnimation = (MeleeAnimation)itemAnimation;
                }

                if (consumable && !consumableAnimation)
                {
                    //attempt to cast from the item animation
                    consumableAnimation = (ConsumableAnimation)itemAnimation;
                }
            }
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
                if (!firearm && !throwable && !consumable && !placeable)
                {
                    melee.CheckInputs(meleeInput, previousMeleeInput, rightMouseInput);
                }

                else
                {
                    melee.CheckInputs(meleeInput, previousMeleeInput, rightMouseInput);

                    if (firearm)
                    {
                        //if we're running, only check for reload input, so that we cannot aim or gun on the run
                        if (playerController && playerController.GetMovementState() == PlayerMovementState.RUNNING)
                        {
                            firearm.CheckInputs(false, false, reloadInput);
                        }

                        else firearm.CheckInputs(leftMouseInput, rightMouseInput, reloadInput);
                    }

                    else if (consumable) consumable.CheckInputs(rightMouseInput);

                    else if (throwable) throwable.CheckInputs();

                    else if (placeable) placeable.CheckInputs();
                }
            }

            else if (!melee)
            {
                if (firearm)
                {
                    //if we're running, only check for reload input, so that we cannot aim or gun on the run
                    if (playerController && playerController.GetMovementState() == PlayerMovementState.RUNNING)
                    {
                        firearm.CheckInputs(false, false, reloadInput);
                    }

                    else firearm.CheckInputs(leftMouseInput, rightMouseInput, reloadInput);
                }

                else if (consumable) consumable.CheckInputs(rightMouseInput);

                else if (throwable) throwable.CheckInputs();

                else if (placeable) placeable.CheckInputs();
            }

            Animation();
        }

        protected void Animation()
        {
            if (itemAnimation)
            {
                //if we're running, do the running animations
                if (playerController && playerController.GetMovementState() == PlayerMovementState.RUNNING)
                {
                    //for melee
                    if (melee && meleeAnimation)
                    {
                        //we need to check if we're attacking or charging an attack
                        //so that way we can do melee attacks whilst on the run...

                        //if we're charging or are charged, play this anim
                        if (melee.GetMeleeState() == MeleeState.CHARGING)
                        {
                            meleeAnimation.PlayMeleeChargeAttack();
                            return;
                        }

                        //if we're blocking and we're not attacking, play this anim
                        else if (melee.GetMeleeState() == MeleeState.BLOCKING)
                        {
                            meleeAnimation.PlayMeleeBlock();
                            return;
                        }

                        //if our damage is the light attack damage, play this anim
                        if (melee.GetMeleeState() == MeleeState.LIGHTATTACKING)
                        {
                            meleeAnimation.PlayMeleeLightAttack();
                            return;
                        }

                        //if our damage is the heavy attack damage, play this anim
                        else if (melee.GetMeleeState() == MeleeState.HEAVYATTACKING)
                        {
                            meleeAnimation.PlayMeleeHeavyAttack();
                            return;
                        }

                        //if our damage is the exhausted attack damage, play this anim
                        else if (melee.GetMeleeState() == MeleeState.EXHAUSTEDATTACKING)
                        {
                            meleeAnimation.PlayMeleeExhaustedAttack();
                            return;
                        }
                    }

                    itemAnimation.PlayRunning();
                    return;
                }

                //MELEE
                if (melee && meleeAnimation)
                {
                    //if we're charging or are charged, play this anim
                    if (melee.GetMeleeState() == MeleeState.CHARGING)
                    {
                        meleeAnimation.PlayMeleeChargeAttack();
                        return;
                    }

                    //if we're blocking and we're not attacking, play this anim
                    else if (melee.GetMeleeState() == MeleeState.BLOCKING)
                    {
                        meleeAnimation.PlayMeleeBlock();
                        return;
                    }

                    //if our damage is the light attack damage, play this anim
                    if (melee.GetMeleeState() == MeleeState.LIGHTATTACKING)
                    {
                        meleeAnimation.PlayMeleeLightAttack();
                        return;
                    }

                    //if our damage is the heavy attack damage, play this anim
                    else if (melee.GetMeleeState() == MeleeState.HEAVYATTACKING)
                    {
                        meleeAnimation.PlayMeleeHeavyAttack();
                        return;
                    }

                    //if our damage is the exhausted attack damage, play this anim
                    else if (melee.GetMeleeState() == MeleeState.EXHAUSTEDATTACKING)
                    {
                        meleeAnimation.PlayMeleeExhaustedAttack();
                        return;
                    }

                    //if we're not attacking, play this anim
                    else if (melee.GetMeleeState() == MeleeState.IDLE)
                    {
                        meleeAnimation.PlayIdle();
                        return;
                    }
                }

                //FIREARM
                if (firearm && firearmAnimation)
                {
                    //if we're cocking the gun, then play this anim
                    if (firearm.GetFirearmState() == FirearmState.COCKING)
                    {
                        firearmAnimation.PlayFirearmCock();
                        return;
                    }

                    //if we're reloading the gun, then play this anim
                    else if (firearm.GetFirearmState() == FirearmState.RELOADING)
                    {
                        firearmAnimation.PlayFirearmReload();
                        return;
                    }

                    //if we're aiming the gun and we're not shooting, play this anim
                    else if (firearm.GetFirearmState() == FirearmState.AIMING)
                    {
                        firearmAnimation.PlayFirearmAim();
                        return;
                    }

                    //if we're aiming the gun and we're shooting, play this anim
                    else if (firearm.GetFirearmState() == FirearmState.AIMATTACK)
                    {
                        firearmAnimation.PlayFirearmAimFire();
                        return;
                    }

                    //if we're shooting, play this anim
                    else if (firearm.GetFirearmState() == FirearmState.ATTACKING)
                    {
                        firearmAnimation.PlayFirearmFire();
                        return;
                    }

                    //ITEM
                    //if we're not attacking, play this anim
                    else if (firearm.GetFirearmState() == FirearmState.IDLE)
                    {
                        firearmAnimation.PlayIdle();
                        return;
                    }
                }

                //CONSUMABLE
                if (consumable && consumableAnimation)
                {
                    if (consumable.GetConsumableState() == ConsumableState.CONSUMING)
                    {
                        consumableAnimation.PlayConsumableConsuming();
                    }

                    else if (consumable.GetConsumableState() == ConsumableState.IDLE)
                    {
                        consumableAnimation.PlayIdle();
                    }
                }
            }
        }
    }
}