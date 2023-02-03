using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [System.Serializable]
    public enum ConsumableType { FOOD, DRINK, MEDICINE, DRUG }

    public class Consumable : Item
    {
        private ConsumableData consumableData;

        private ConsumableType consumableType;

        private int useAmount;
        private int consumableAmount;
        private float consumeTime;

        private ConsumableSound consumableSound;

        private WeaponSway weaponSway;
        private PlayerStats playerStats;

        protected bool isConsuming, leftMouseInput, rightMouseInput;
        protected int remainingUsageAmount = -1, remainingConsumableAmount = -1;

        protected override void Init()
        {
            InitStats(); InitEffects();

            if (!playerStats) playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        }

        private void InitStats()
        {
            //if we have no item data assigned
            if (!itemData)
            {
                Debug.LogWarning("No Item Data found for " + gameObject.name + "; Assign Item Data!");
                return;
            }

            //if we have item data assigned
            else if (itemData)
            {
                //if we have no consumable data
                if (!consumableData)
                {
                    //then cast from our item data
                    consumableData = (ConsumableData)itemData;
                }
            }

            itemName = consumableData.itemName;
            itemWeight = consumableData.itemWeight;

            useAmount = consumableData.useAmount;

            consumableType = consumableData.consumableType;
            consumableAmount = consumableData.consumableAmount;
            consumeTime = consumableData.consumeTime;
        }

        private void InitEffects()
        {
            //if we have no item sound assigned
            if (!itemSound) Debug.LogWarning("No Item Sound found for " + gameObject.name + "; Assign Sound Reference!");

            //if we have item sound assigned
            else if (itemSound)
            {
                //if we have no consumable sound
                if (!consumableSound)
                {
                    //then cast from our item sound
                    consumableSound = (ConsumableSound)itemSound;
                }
            }

            //if we have no item sway assigned
            if (!itemSway) Debug.LogWarning("No Item Sway found for " + gameObject.name + "; Assign Sway Reference!");

            //if we have item sway assigned
            else if (itemSway)
            {
                //if we have no consumable sway
                if (!weaponSway)
                {
                    //then cast from our item sway
                    weaponSway = (WeaponSway)itemSway;
                }
            }
        }

        public override void ToggleEquip(bool b)
        {
            isEquipped = b;

            //if we have equipped the consumable, play the equip sound.
            if (isEquipped && consumableSound)
            {
                consumableSound.PlayEquipSound();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            //if we haven't initialized the usage remaining, do it here.
            if (remainingUsageAmount == -1) remainingUsageAmount = useAmount;

            //if we haven't initialized the consumable remaining, do it here.
            if (remainingConsumableAmount == -1) remainingConsumableAmount = consumableAmount;
        }

        //Used for Initialization
        void OnEnable()
        {
            //if we haven't initialized the usage remaining, do it here.
            if (remainingUsageAmount == -1) remainingUsageAmount = useAmount;

            //if we haven't initialized the consumable remaining, do it here.
            if (remainingConsumableAmount == -1) remainingConsumableAmount = consumableAmount;
        }

        // Update is called once per frame
        void Update() => Inputs();

        protected virtual void Inputs()
        {
            if (Input.GetMouseButtonDown(0)) leftMouseInput = true;
            else if (Input.GetMouseButtonUp(0)) leftMouseInput = false;

            if (Input.GetMouseButtonDown(1)) rightMouseInput = true;
            else if (Input.GetMouseButtonUp(1)) rightMouseInput = false;

            CheckConsume();
        }

        protected virtual void CheckConsume()
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

        protected virtual void StartConsume()
        {
            isConsuming = true;

            Aim(isConsuming);

            //invoke the consume method after the consume time
            Invoke(nameof(Consume), consumeTime);

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
                    //round to an int how much hunger we're gonna consume
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

        public override ItemData GetItemData()
        {
            if (consumableData != null) return consumableData;

            else return null;
        }

        public override void SetItemData(ItemData itemData)
        {
            consumableData = (ConsumableData)itemData;

            Init();
        }
    }
}