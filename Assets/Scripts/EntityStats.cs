using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class EntityStats : MonoBehaviour
    {
        [Header("Variables - Health")]
        [SerializeField] protected int maxHealth = 100;
        [SerializeField] protected int currentHealth;

        [Space]
        [SerializeField] protected bool isDead;


        //later down the line we should do this max stuff in scriptable objects instead...
        //then just load in the stats from that scriptable object...

        private void Start()
        {
            InitVariables();
        }

        protected virtual void InitVariables()
        {
            SetCurrentHealth(maxHealth);

            isDead = false;
        }

        //returns our current health
        //(maybe replace with events instead?)
        public int GetCurrentHealth()
        {
            return currentHealth;
        }

        //sets our current health
        public void SetCurrentHealth(int amount)
        {
            currentHealth = amount;

            //doing our clamping in here
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }

        //a method to deplete health
        public void DepleteHealth(int amount)
        {
            //decreases current health
            SetCurrentHealth(GetCurrentHealth() - amount);

            //checking if our health is 0
            if (GetCurrentHealth() <= 0)
            {
                Kill(); //kill this character
            }
        }

        //a method to replenish health
        public void ReplenishHealth(int amount)
        {
            //increaes current health
            SetCurrentHealth(GetCurrentHealth() + amount);

            if (GetCurrentHealth() >= maxHealth) //if we have full health
            {
                //then debug log that we have full health
                Debug.Log("Character (" + gameObject.name + ") is at Full Health");
            }
        }

        //a method to kill this entity
        protected virtual void Kill()
        {
            //currently we're only debug logging lmao.
            Debug.Log("Character (" + gameObject.name + ") is Dead");
        }
    }
}