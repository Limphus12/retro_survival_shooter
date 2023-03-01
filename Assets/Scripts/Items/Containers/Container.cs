using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public abstract class Container : MonoBehaviour, IInteractable
    {
        [Header("Attributes - Container")]
        [SerializeField] protected int lootAmount; //How many times this container can be looted
        [SerializeField] protected float lootTime; //How long it takes to loot this container once

        [Space]
        [SerializeField] protected PlayerInventory playerInventory;
        [SerializeField] protected ItemType containerType;

        [Space]
        [SerializeField] protected ContainerAnimation containerAnimation;
        [SerializeField] protected ContainerSound containerSound;

        protected bool isLooting, isInteracting;
        protected int remainingLootAmount = -1;

        public abstract void StartInteract();
        public abstract void StopInteract();

        public ItemType GetItemType() => containerType;

        public void SetItemType(ItemType containerType) => this.containerType = containerType;

        protected void Awake() => Init();

        protected virtual void Init()
        {
            if (remainingLootAmount == -1) remainingLootAmount = lootAmount; //if we haven't initialized the loot amount remaining, do it here.

            if (!playerInventory) playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();

            if (!containerAnimation) containerAnimation = GetComponent<ContainerAnimation>();
            if (!containerSound) containerSound = GetComponent<ContainerSound>();
        }

        public abstract bool CanLoot();
        public abstract bool IsLooting();
        public abstract void StartLoot();
        protected abstract void Loot();
        protected abstract void EndLoot();
        public abstract void StopLoot();
    }
}