using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [System.Serializable]
    public enum ConsumableType { FOOD, DRINK, MEDICINE, DRUG }

    public enum ConsumableState { IDLE, CONSUMING }

    public class Consumable : MonoBehaviour
    {
        [Header("Attributes - Consumable")]
        [SerializeField] private ConsumableData consumableData;

        private ConsumableType consumableType;

        private int useAmount;
        private int consumableAmount;
        private float consumeTime;

        private ConsumableSound consumableSound;
        private WeaponSway weaponSway;

        protected bool isConsuming;
        protected int remainingUsageAmount = -1, remainingConsumableAmount = -1;

        private PlayerStats playerStats;

        private void Awake() => Init();

        private void Init()
        {
            InitStats(); InitEffects();

            //if we haven't initialized the usage remaining, do it here.
            if (remainingUsageAmount == -1) remainingUsageAmount = useAmount;

            //if we haven't initialized the consumable remaining, do it here.
            if (remainingConsumableAmount == -1) remainingConsumableAmount = consumableAmount;

            if (!playerStats) playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        }

        private void InitStats()
        {
            useAmount = consumableData.useAmount;

            consumableType = consumableData.consumableType;
            consumableAmount = consumableData.consumableAmount;
            consumeTime = consumableData.consumeTime;
        }

        private void InitEffects()
        {
            if (!consumableSound) consumableSound = gameObject.GetComponent<ConsumableSound>();

            if (!weaponSway) weaponSway = gameObject.GetComponent<WeaponSway>();
        }

        public void CheckInputs(bool rightMouseInput)
        {
            if (playerStats)
            {
                //if we're not consuming atm
                if (!isConsuming)
                {
                    //and we are holding our right mouse button
                    if (rightMouseInput && remainingUsageAmount > 0)
                    {
                        //start consuming
                        StartConsume();
                    }

                    //if we are holding the right mouse button but we cannot consume it
                    else if (rightMouseInput && remainingUsageAmount == 0)
                    {
                        Debug.Log("We cannot consume this consumable, we've already consumed it all!");
                    }
                }

                //if we're currently consuming
                else if (isConsuming)
                {
                    //if we release our right mouse input
                    if (!rightMouseInput)
                    {
                        //stop the invoke of the Consume method
                        CancelInvoke(nameof(Consume));

                        //and end our consuming
                        EndConsume();
                    }
                }
            }

            //if we dont have the player stats reference
            else if (!playerStats)
            {
                Debug.LogWarning("No Player Stats Detected! Please Assign the Player Stats!");
                return;
            }
        }

        protected virtual void StartConsume()
        {
            isConsuming = true;

            Aim(isConsuming);

            //invoke the consume method after the consume time
            Invoke(nameof(Consume), consumeTime);

            if (consumableSound) consumableSound.PlayConsumingSound();

            Debug.Log("Started Consuming");
        }

        protected virtual void Consume()
        {
            //check for the player stats script and increase a certain stat
            if (playerStats)
            {
                //do some maths to figure out how much of the sustenance amount is used
                //sustenance usage = sustenance amount / consume amount
                //gotta round it to an int since we track hunger as an int

                //if we're not on the final consume, do the maths
                if (remainingUsageAmount > 1)
                {
                    //round to an int how much consumableAmount we're gonna use
                    int i = Mathf.RoundToInt(consumableAmount / useAmount);

                    //switch statement to replenish either hunger or thirst
                    switch (consumableType)
                    {
                        case ConsumableType.FOOD:
                            //call the replenish hunger method on our player stats reference
                            playerStats.ReplenishHunger(i);
                            break;

                        case ConsumableType.DRINK:
                            //call the replenish thirst method on our player stats reference
                            playerStats.ReplenishThirst(i);
                            break;

                        case ConsumableType.MEDICINE:
                            //call the replenish health method on our player stats reference
                            playerStats.ReplenishHealth(i);
                            break;

                        case ConsumableType.DRUG:
                            //TODO - Probably make a drug class that extends from here?
                            break;

                        default:
                            break;
                    }

                    //decreases our remaining sustenance amount
                    remainingConsumableAmount -= i;
                }

                //if we're on our last consume, just consume the rest of it, no maths required
                else if (remainingUsageAmount == 1)
                {
                    //switch statement to replenish either hunger or thirst
                    switch (consumableType)
                    {
                        case ConsumableType.FOOD:
                            //call the replenish hunger method on our player stats reference
                            playerStats.ReplenishHunger(remainingConsumableAmount);
                            break;

                        case ConsumableType.DRINK:
                            //call the replenish thirst method on our player stats reference
                            playerStats.ReplenishThirst(remainingConsumableAmount);
                            break;

                        case ConsumableType.MEDICINE:
                            //call the replenish health method on our player stats reference
                            playerStats.ReplenishHealth(remainingConsumableAmount);
                            break;

                        case ConsumableType.DRUG:
                            //TODO - Probably make a drug class that extends from here?
                            break;

                        default:
                            break;
                    }

                    //decreases our remaining sustenance amount
                    remainingConsumableAmount = 0;
                }
            }

            //decrease the remaining consume amount
            remainingUsageAmount--;

            //if we have the sustenance sound reference, play the consume sound
            if (consumableSound)
            {
                consumableSound.PlayConsumeSound();
            }

            //call the end consume menthod
            EndConsume();

            Debug.Log("Consuming");
        }

        protected virtual void EndConsume()
        {
            isConsuming = false;

            Aim(isConsuming);

            Debug.Log("Ended Consuming");
        }

        private void Aim(bool b)
        {
            //if we have the weapon sway reference, call the aim method on it too
            if (weaponSway) weaponSway.Aim(b);
        }

        public ConsumableState GetConsumableState()
        {
            if (isConsuming) return ConsumableState.CONSUMING;
            else return ConsumableState.IDLE;
        }

        public ConsumableData GetConsumableData()
        {
            if (consumableData != null) return consumableData;

            else return null;
        }

        public void SetConsumableData(ConsumableData consumableData)
        {
            this.consumableData = consumableData;

            Init();
        }
    }
}