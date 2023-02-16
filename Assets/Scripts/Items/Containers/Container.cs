using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public abstract class Container : MonoBehaviour
    {
        [Header("Attributes - Container")]
        [SerializeField] protected int lootAmount; //How many times this container can be looted
        [SerializeField] protected float lootTime; //How long it takes to loot this container once

        [Space]
        [SerializeField] protected PlayerInventory playerInventory;

        protected bool isLooting;
        protected int remainingLootAmount = -1;

        protected void Awake() => Init();

        protected virtual void Init()
        {
            if (remainingLootAmount == -1) remainingLootAmount = lootAmount; //if we haven't initialized the loot amount remaining, do it here.

            if (!playerInventory) playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerInventory>();
        }


        protected abstract void CheckLoot();
        protected abstract void StartLoot();
        protected abstract void Loot();
        protected abstract void EndLoot();
    }
}