using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class CharacterStats : MonoBehaviour
    {
        [Header("Attributes - Health")]
        [SerializeField] protected int maxHealth = 100;

        //later down the line we should do this stuff in scriptable objects instead...
        //then just load in the stats from that scriptable object...

        [SerializeField] protected int currentHealth;
        [SerializeField] protected bool isDead;


        private void Start()
        {
            InitVariables();
        }

        private void InitVariables()
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
        }

        //a method to deplete health
        public void DepleteHealth(int amount)
        {
            //decreases current health
            currentHealth -= amount;

            //clamping the health between 0 and max health
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            //checking if our health is 0
            if (currentHealth <= 0)
            {
                Kill(); //kill this character
            }
        }

        //a method to replenish health
        public void ReplenishHealth(int amount)
        {
            currentHealth += amount; //increaes current health

            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); //clamping the health between 0 and max health

            
            if (currentHealth >= maxHealth) //if we have full health
            {
                //then debug log that we have full health
                Debug.Log("Character (" + gameObject.name + ") is at Full Health");
            }
        }

        //a method to kill this character
        protected void Kill()
        {
            //currently we're only debug logging lmao.
            Debug.Log("Character (" + gameObject.name + ") is Dead");
        }
    }
}