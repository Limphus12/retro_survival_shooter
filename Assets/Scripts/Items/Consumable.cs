using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public abstract class Consumable : Item
    {
        [Header("Attributes - Consumable")]
        [Tooltip("How many times you can Consume this item")] [SerializeField] protected int consumeAmount;
        [Tooltip("How long it takes to Consume this item")] [SerializeField] protected float consumeTime;

        protected bool isConsuming, leftMouseInput, rightMouseInput;

        protected int remainingConsumeAmount = -1;

        //NOTE - I would've put the initializations for the remaining consume amount in this class
        //but at runtime this was not allowing initialization. i guess abstract classes cannot
        //call unity methods? interesting nonetheless, but now those initializations are in the
        //classes that inherit from this class.

        protected abstract void Inputs();
        protected abstract void CheckConsume();
        protected abstract void StartConsume();
        protected abstract void Consume();
        protected abstract void EndConsume();
    }
}