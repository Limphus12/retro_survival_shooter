using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.limphus.utilities;

namespace com.limphus.retro_survival_shooter
{
    public class EntityStats : MonoBehaviour, IDamageable
    {
        [Header("Variables - Health")]
        [SerializeField] protected int maxHealth = 100;
        protected int currentHealth;

        public bool IsDead { get; private set; }

        //later down the line we should do this max stuff in scriptable objects instead...
        //then just load in the stats from that scriptable object...

        public class OnTemperatureChangedEventArgs : EventArgs { public Temperature i; }

        public event EventHandler<Events.OnIntChangedEventArgs> OnHealthChanged;
        public event EventHandler<EventArgs> OnHealthDepleted, OnHealthReplenished;

        private void Awake() => InitVariables();

        protected virtual void InitVariables()
        {
            SetCurrentHealth(maxHealth);

            IsDead = false;
        }

        public bool CanReplenishHealth()
        {
            if (currentHealth < maxHealth) return true;
            else return false;
        }

        //returns our current health
        //(maybe replace with events instead?)
        public int GetCurrentHealth () => currentHealth;
        public int GetMaxHealth() => maxHealth;

        //sets our current health
        public void SetCurrentHealth(int amount)
        {
            currentHealth = amount;

            //doing our clamping in here
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            //firing off our event here
            OnHealthChanged?.Invoke(this, new Events.OnIntChangedEventArgs { i = currentHealth });

            //check our health
            CheckHealth();
        }

        //a method to deplete health
        public void Damage(int amount)
        {
            if (IsDead) return;

            //decreases current health
            SetCurrentHealth(GetCurrentHealth() - amount);

            OnHealthDepleted?.Invoke(this, new EventArgs { });

            //check our health
            CheckHealth();
        }

        //a method to check our current health
        void CheckHealth()
        {
            //checking if our health is 0
            if (GetCurrentHealth() <= 0)
            {
                Kill(); //kill this character
            }

            else if (GetCurrentHealth() >= maxHealth) //if we have full health
            {
                //then debug log that we have full health
                //Debug.Log("Character (" + gameObject.name + ") is at Full Health");
            }
        }

        //a method to replenish health
        public void ReplenishHealth(int amount)
        {
            //increaes current health
            SetCurrentHealth(GetCurrentHealth() + amount);

            //replenish event
            OnHealthReplenished?.Invoke(this, new EventArgs { });

            //check our health
            CheckHealth();
        }

        //a method to kill this entity
        protected virtual void Kill()
        {
            IsDead = true;

            //currently we're only debug logging lmao.
            Debug.Log("Character (" + gameObject.name + ") is Dead");
        }
    }
}