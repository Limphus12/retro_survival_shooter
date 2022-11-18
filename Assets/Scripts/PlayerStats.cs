using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public enum Tempurature { VERY_COLD, COLD, NORMAL, HOT, VERY_HOT }

    public class PlayerStats : CharacterStats
    {
        [Header("Attributes - Player Survival - Starting Stats")]
        [SerializeField] private int maxHunger;
        [SerializeField] private int maxThirst, maxStamina;

        [SerializeField] private Tempurature startingTempurature;

        [Header("Attributes - Player Survival - Current Stats")]
        [SerializeField] private int currentHunger;
        [SerializeField] private int currentThirst, currentStamina;

        [Space]
        [SerializeField] private Tempurature currentTempurature;



        [Header("Attributes - Player Survival - Hunger & Thirst Depletion")]
        [Tooltip("[IN SECONDS] - How quickly Hunger depletes")] [SerializeField] private float hungerTickRate;
        [Tooltip("[IN SECONDS] - How quickly Thirst depletes")] [SerializeField] private float thirstTickRate;

        [Space]
        [Tooltip("How much Hunger is depleted per tick")] [SerializeField] private int hungerDepletionRate;
        [Tooltip("How much Thirst is depleted per tick")] [SerializeField] private int thirstDepletionRate;



        [Header("Attributes - Player Survival - Hunger & Thirst Damage")]
        [Tooltip("[IN SECONDS] - How quickly damage occurs when at 0 Hunger")] [SerializeField] private float hungerDepletedTickRate;
        [Tooltip("[IN SECONDS] - How quickly damage occurs when at 0 Thirst")] [SerializeField] private float thirstDepletedTickRate;

        [Space]
        [Tooltip("How much Damage is depleted per tick when at 0 Hunger")] [SerializeField] private int hungerDepletedDamage;
        [Tooltip("How much Damage is depleted per tick when at 0 Thirst")] [SerializeField] private int thirstDepletedDamage;

        private void Start()
        {
            //firstly cancel all invokes
            CancelInvoke();

            //then invoke our repeating hunger and thirst ticks
            InvokeRepeating(nameof(HungerTick), 0f, hungerTickRate);
            InvokeRepeating(nameof(ThirstTick), 0f, thirstTickRate);
        }

        #region Hunger

        //used to grab our current hunger
        //(maybe replace with events instead?)
        public int GetCurrentHunger()
        {
            return currentHunger;
        }

        //sets our current hunger
        public void SetCurrentHunger(int amount)
        {
            currentHunger = amount;
        }

        //a method to deplete hunger
        public void DepleteHunger(int amount)
        {
            //decreases current hunger
            currentHunger -= amount;

            //clamping the hunger between 0 and max hunger
            currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);

            //checking if our hunger is 0
            if (currentHunger <= 0)
            {
                //we have to invoke a differnt method (hungerdepletedtick) that then calls the depleteHealth method
                //cos invokes cannot pass through paramaters
                if (!IsInvoking(nameof(HungerDepletedTick)))
                {
                    InvokeRepeating(nameof(HungerDepletedTick), 0f, hungerDepletedTickRate);

                }
            }
        }

        //a method to replenish hunger
        public void ReplenishHunger(int amount)
        {
            currentHunger += amount; //increaes current hunger

            currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger); //clamping the hunger between 0 and max hunger

            //if we're above 0 hunger
            if (currentHunger > 0)
            {
                //cancel the hunger depleted tick
                CancelInvoke(nameof(HungerDepletedTick));
            }

            if (currentHunger >= maxHunger) //if we have full hunger
            {
                //then debug log that we have full hunger
                Debug.Log("Character (" + gameObject.name + ") is at Full Hunger");
            }
        }

        #endregion

        #region Thirst

        //used to grab our current thirst
        //(maybe replace with events instead?)
        public int GetCurrentThirst()
        {
            return currentThirst;
        }

        //sets our current thirst
        public void SetCurrentThirst(int amount)
        {
            currentThirst = amount;
        }

        //a method to deplete thirst
        public void DepleteThirst(int amount)
        {
            //decreases current thirst
            currentThirst -= amount;

            //clamping the thirst between 0 and max thirst
            currentThirst = Mathf.Clamp(currentThirst, 0, maxThirst);

            //checking if our thirst is 0
            if (currentThirst <= 0)
            {
                //we have to invoke a differnt method (thirstdepletedtick) that then calls the depleteHealth method
                //cos invokes cannot pass through paramaters
                if (!IsInvoking(nameof(ThirstDepletedTick)))
                {
                    InvokeRepeating(nameof(ThirstDepletedTick), 0f, thirstDepletedTickRate);
                }
            }
        }

        //a method to replenish thirst
        public void ReplenishThirst(int amount)
        {
            currentThirst += amount; //increaes current health

            currentThirst = Mathf.Clamp(currentThirst, 0, maxThirst); //clamping the health between 0 and max health

            //if we're above 0 thirst
            if (currentThirst > 0)
            {
                //cancel the thirst depleted tick
                CancelInvoke(nameof(ThirstDepletedTick));
            }

            if (currentThirst >= maxThirst) //if we have full thirst
            {
                //then debug log that we have full thirst
                Debug.Log("Character (" + gameObject.name + ") is at Full Thirst");
            }
        }

        #endregion

        #region Ticks

        //regular ticks - over time, hunger and thirst are depleted
        private void HungerTick()
        {
            DepleteHunger(hungerDepletionRate);
        }

        private void ThirstTick()
        {
            DepleteThirst(thirstDepletionRate);
        }

        //damage ticks - when hunger and thirst are depleted
        private void HungerDepletedTick()
        {
            DepleteHealth(hungerDepletedDamage);
        }

        private void ThirstDepletedTick()
        {
            DepleteHealth(thirstDepletedDamage);
        }

        #endregion
    }
}