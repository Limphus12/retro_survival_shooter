using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    [System.Serializable]
    public enum ConsumableType { FOOD, DRINK, MEDICINE, DRUG }

    public class Consumable : Melee
    {
        private ConsumableData consumableData;

        private ConsumableType consumableType;

        private int useAmount;
        private int consumableAmount;
        private float consumeTime;

        private ConsumableSound consumableSound;

        protected bool isConsuming;
        protected int remainingUsageAmount = -1, remainingConsumableAmount = -1;

        protected override void Init()
        {
            InitStats(); InitEffects();

            if (!playerStats) playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        }

        protected override void InitStats()
        {
            base.InitStats();

            //if we have item data assigned
            if (itemData)
            {
                //if we have no firearm data
                if (!consumableData)
                {
                    //cast the consumable data from our item data
                    consumableData = (ConsumableData)itemData;
                }
            }

            //CONSUMABLE
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

        protected override void Inputs()
        {
            if (Input.GetMouseButtonDown(0)) leftMouseInput = true;
            else if (Input.GetMouseButtonUp(0)) leftMouseInput = false;

            if (Input.GetMouseButtonDown(1)) rightMouseInput = true;
            else if (Input.GetMouseButtonUp(1)) rightMouseInput = false;

            if (Input.GetKeyDown(KeyCode.V)) meleeInput = true;
            else if (Input.GetKeyUp(KeyCode.V)) meleeInput = false;

            CheckInputs();
        }

        protected virtual void CheckInputs()
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

                //check for stamina
                float stamina = playerStats.GetCurrentStamina();

                //if were not attacking and we press the l-mouse button or melee input
                if (meleeInput)
                {
                    //So in code, ima have to check if we have mouse input, either start a timer or invoke some methods, then if we let go of the mouse before the specified time,
                    //cancel the timer/invoke and do a light attack. However, if we go beyond the timer or we don't cancel the method invoke (because we held down the mouse),
                    //then do a heavy attack. All before this, however, we need to check if we even have any stamina,
                    //because if we don't, then we should perform an exhausted attack. Yeah, that sounds good…

                    //if we're already charged up/are charging up, just return out of this, since we can only attack when we release the mouse button
                    if (isCharged || isCharging) return;

                    //if we are invoking the charge function
                    if (IsInvoking(nameof(Charge)))
                    {
                        //if we're not charged or not charging (which should be an impossibility
                        if (!isCharged && !isCharging)
                        {
                            Debug.LogWarning("calling the Charge function, even though we are not charged or charging?! this may be a bug");
                        }

                        //else just return, since we should be charging
                        else return;
                    }

                    //else if we're not invoking the charge, and we have not already charged, and we have stamina
                    else if (!IsInvoking(nameof(Charge)) && !isCharged && !isCharging && stamina > 0)
                    {
                        StartCharge(); return;
                    }

                    else if (stamina <= 0)
                    {
                        //do an exhausted attack!
                        Debug.Log("Exhausted Attack!");
                        SetupAttack(exhaustedAttackTimeToHit, exhaustedAttackRate, exhaustedAttackDamage, 0);
                    }
                }

                //if we release the left mouse button or melee input
                else if (!meleeInput)
                {
                    //if we we're have not been holding down the mouse
                    if (previousMeleeInput == meleeInput)
                    {
                        //call the reset charge method, just in case
                        ResetCharge();
                    }

                    //if we were holding down the mouse in teh last frame
                    else if (previousMeleeInput != meleeInput)
                    {
                        //if we have charged
                        if (isCharged)
                        {
                            //do a heavy attack!
                            Debug.Log("Heavy Attack!");
                            SetupAttack(heavyAttackTimeToHit, heavyAttackRate, heavyAttackDamage, heavyAttackStaminaCost);
                        }

                        //if we have not charged
                        else if (!isCharged)
                        {
                            //if we are charging
                            if (isCharging)
                            {
                                //cancel the charge
                                CancelInvoke(nameof(Charge));
                            }

                            //if we have stamina
                            if (stamina > 0)
                            {
                                //do the light attack
                                Debug.Log("Light Attack!");
                                SetupAttack(lightAttackTimeToHit, lightAttackRate, lightAttackDamage, lightAttackStaminaCost);
                            }
                        }

                        //and call the reset charge method
                        ResetCharge();
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

        protected override void Animation()
        {
            if (meleeAnimation)
            {
                //MELEE

                //if we're charging our heavy attack, then play this anim
                if (isCharged || isCharging)
                {
                    meleeAnimation.PlayMeleeChargeAttack();
                    return;
                }

                //if we're attacking in melee
                else if (isAttacking && meleeAttack)
                {
                    //if our damage is the light attack damage, play this anim
                    if (currentDamage == lightAttackDamage)
                    {
                        meleeAnimation.PlayMeleeLightAttack();
                        return;
                    }

                    //if our damage is the heavy attack damage, play this anim
                    else if (currentDamage == heavyAttackDamage)
                    {
                        meleeAnimation.PlayMeleeHeavyAttack();
                        return;
                    }

                    //if our damage is the exhausted attack damage, play this anim
                    else if (currentDamage == exhaustedAttackDamage)
                    {
                        meleeAnimation.PlayMeleeExhaustedAttack();
                        return;
                    }
                }

                //ITEM

                //if we're not attacking, play this anim
                else if (!isAttacking)
                {
                    meleeAnimation.PlayIdle();
                    return;
                }
            }
        }


        private void Aim(bool b)
        {
            //if we have the weapon sway reference, call the aim method on it too
            if (weaponSway) weaponSway.Aim(b);
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