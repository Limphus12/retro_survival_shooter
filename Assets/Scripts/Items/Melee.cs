using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class Melee : Weapon
    {
        private MeleeData meleeData;

        private float attackRange;

        private float lightAttackTimeToHit;
        private int lightAttackStaminaCost;

        private float heavyAttackRate;
        private float heavyAttackDamage, chargeUpTime, heavyAttackTimeToHit;
        private int heavyAttackStaminaCost;

        private float exhaustedAttackRate;
        private float exhaustedAttackDamage, exhaustedAttackTimeToHit;

        private PlayerStats playerStats;

        private MeleeSound meleeSound;

        private WeaponSway weaponSway;
        private MeleeAnimation meleeAnimation;

        private bool isBlocking, isCharging, isCharged, previousLeftMouseInput;

        //initialization
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
                //then cast from our item data
                Debug.LogWarning("No Item Data found for " + gameObject.name + "; Assign Item Data!");
                return;
            }

            //if we have item data assigned
            else if (itemData)
            {
                //if we have no melee data
                if (!meleeData)
                {
                    meleeData = (MeleeData)itemData;
                }
            }

            itemName = meleeData.itemName;
            itemWeight = meleeData.itemWeight;

            damage = meleeData.damage;
            attackRate = meleeData.attackRate;

            attackRange = meleeData.attackRange;
            lightAttackTimeToHit = meleeData.lightAttackTimeToHit;
            lightAttackStaminaCost = meleeData.lightAttackStaminaCost;

            heavyAttackRate = meleeData.heavyAttackRate;
            heavyAttackDamage = meleeData.heavyAttackDamage;
            chargeUpTime = meleeData.chargeUpTime;
            heavyAttackTimeToHit = meleeData.heavyAttackTimeToHit;
            heavyAttackStaminaCost = meleeData.heavyAttackStaminaCost;

            exhaustedAttackRate = meleeData.exhaustedAttackRate;
            exhaustedAttackDamage = meleeData.exhaustedAttackDamage;
            exhaustedAttackTimeToHit = meleeData.exhaustedAttackTimeToHit;
        }

        private void InitEffects()
        {
            //if we have no item sound assigned
            if (!itemSound) Debug.LogWarning("No Item Sound found for " + gameObject.name + "; Assign Sound Reference!");

            //if we have item sound assigned
            else if (itemSound)
            {
                //if we have no melee sound
                if (!meleeSound)
                {
                    //then cast from our item sound
                    meleeSound = (MeleeSound)itemSound;
                }
            }

            //if we have no item sway assigned
            if (!itemSway) Debug.LogWarning("No Item Sway found for " + gameObject.name + "; Assign Sway Reference!");

            //if we have item sway assigned
            else if (itemSway)
            {
                //if we have no melee sway
                if (!weaponSway)
                {
                    //then cast from our item sway
                    weaponSway = (WeaponSway)itemSway;
                }
            }

            //if we have no item animation assigned
            if (!itemAnimation) Debug.LogWarning("No Item Animation found for " + gameObject.name + "; Assign Animation Reference!");

            //if we have item animation assigned
            else if (itemAnimation)
            {
                //if we have no melee animation
                if (!meleeAnimation)
                {
                    //then cast from our item animation
                    meleeAnimation = (MeleeAnimation)itemAnimation;
                }
            }
        }


        public override void ToggleEquip(bool b)
        {
            isEquipped = b;

            //if we have equipped the weapon, play the equip sound.
            if (isEquipped && meleeSound)
            {
                meleeSound.PlayEquipSound();
            }
        }

        private void Start()
        {
            //if this weapon is not equipped, then return;
            if (!isEquipped) return;
        }

        private void Update()
        {
            Inputs(); Animation();
        }

        protected override void Inputs()
        {
            previousLeftMouseInput = leftMouseInput;

            if (Input.GetMouseButtonDown(0)) leftMouseInput = true;
            else if (Input.GetMouseButtonUp(0)) leftMouseInput = false;

            if (Input.GetMouseButtonDown(1)) rightMouseInput = true;
            else if (Input.GetMouseButtonUp(1)) rightMouseInput = false;

            CheckAttack();
        }

        protected override void CheckAttack()
        {
            //if were attacking or blocking already, dont do anything
            if (isAttacking) return;

            //determine if we're blocking using the right mouse input
            Block(rightMouseInput);

            //if we are blocking, then dont do anything
            if (isBlocking) return;

            if (playerStats)
            {
                //check for stamina
                float stamina = playerStats.GetCurrentStamina();

                //do exhausted attacks if we press the leftg mouse button and we have 0 stamina
                if (stamina <= 0 && leftMouseInput)
                {
                    //do an exhausted attack!
                    Debug.Log("Exhausted Attack!");
                    SetupAttack(exhaustedAttackTimeToHit, exhaustedAttackRate, exhaustedAttackDamage, 0);
                }

                //do normal attacks if we have more than 0 stamina
                else if (stamina > 0)
                {
                    //if were not attacking and we press the l-mouse button
                    if (leftMouseInput)
                    {
                        //So in code, ima have to check if we have mouse input, either start a timer or invoke some methods, then if we let go of the mouse before the specified time,
                        //cancel the timer/invoke and do a light attack. However, if we go beyond the timer or we don't cancel the method invoke (because we held down the mouse),
                        //then do a heavy attack. All before this, however, we need to check if we even have any stamina,
                        //because if we don't, then we should perform an exhausted attack. Yeah, that sounds good…

                        //if we're already charged up, just return out of this, since we can only attack when we release the mouse button
                        if (isCharged) return;

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

                        //else if we're not invoking the charge, and we have not already charged
                        else if (!IsInvoking(nameof(Charge)) && !isCharged && !isCharging)
                        {
                            StartCharge(); return;
                        }
                    }

                    //if we release the left mouse button
                    else if (!leftMouseInput)
                    {
                        //if we we're have not been holding down the mouse
                        if (previousLeftMouseInput == leftMouseInput)
                        {
                            //call the reset charge method, just in case
                            ResetCharge();
                        }

                        //if we were holding down the mouse in teh last frame
                        else if (previousLeftMouseInput != leftMouseInput)
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

                                //do the light attack
                                Debug.Log("Light Attack!");
                                SetupAttack(lightAttackTimeToHit, attackRate, damage, lightAttackStaminaCost);
                            }

                            //and call the reset charge method
                            ResetCharge();
                        }
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

        private float currentTimeToHit, currentAttackRate, currentDamage;

        //takes in a time to hit, attack rate & damage
        private void SetupAttack(float hitTime, float attackRate, float damage, int staminaCost)
        {
            currentTimeToHit = hitTime;
            currentAttackRate = attackRate;
            currentDamage = damage;

            if (playerStats.GetCurrentStamina() <= staminaCost)
            {
                //reset the stamina replenishment tick & deplete all stamina
                playerStats.ResetStaminaReplenishTick(); playerStats.SetCurrentStamina(0);
            }

            else if (playerStats.GetCurrentStamina() > staminaCost)
            {
                //reset the stamina replenishment tick & deplete a certain amount of stamina
                playerStats.ResetStaminaReplenishTick(); playerStats.DepleteStamina(staminaCost);
            }

            //attack sounds - if we have the melee sound reference
            if (meleeSound)
            {
                if (currentDamage == this.damage)
                {
                    //call the play light attack sound if we are dealing the light attack damage
                    meleeSound.PlayLightAttackSound();
                }

                else if (currentDamage == heavyAttackDamage)
                {
                    //call the play light attack sound if we are dealing the heavy attack damage
                    meleeSound.PlayHeavyAttackSound();
                }

                else if (currentDamage == exhaustedAttackDamage)
                {
                    //call the play light attack sound if we are dealing the exhausted attack damage
                    meleeSound.PlayExhaustedAttackSound();
                }
            }

            StartAttack();
        }

        //starts attacking
        protected override void StartAttack()
        {
            isAttacking = true;

            //invoking attack after a delay to simulate the swinging of a melee weapon
            Invoke(nameof(Attack), currentTimeToHit);

            //invoke end attack after our rate of fire
            Invoke(nameof(EndAttack), 1 / currentAttackRate);
        }

        //shoots!
        protected override void Attack()
        {
            //call the hit function, passing through the player camera
            Hit(playerCamera);
        }

        //ends shooting
        protected override void EndAttack() => isAttacking = false;

        protected override void Hit(Transform point)
        {
            bool hasHit = false;

            //simple raycasting - prolly use this only for stabby knives in teh future?
            RaycastHit hit;
            if (Physics.Raycast(point.position, point.forward, out hit, attackRange))
            {
                IDamageable damageable = hit.transform.GetComponent<IDamageable>();

                if (damageable != null) damageable.Damage(currentDamage);

                hasHit = true;
            }

            //overlap box for melee combat - use in the future for larger swinging weapons
            //Collider[] colliders = Physics.OverlapBox(point.position, Vector3.one);

            //if (colliders.Length <= 0) return;

            //for (int i = 0; i < colliders.Length; i++)
            {
                //IDamageable damageable = colliders[i].transform.GetComponent<IDamageable>();

                //if (damageable != null) damageable.Damage(damage);
            }

            //attack sounds - if we have the melee sound reference
            if (hasHit && meleeSound)
            {
                if (currentDamage == this.damage)
                {
                    //call the play light attack sound if we are dealing the light attack damage
                    meleeSound.PlayLightAttackHitSound();
                }

                else if (currentDamage == heavyAttackDamage)
                {
                    //call the play light attack sound if we are dealing the heavy attack damage
                    meleeSound.PlayHeavyAttackHitSound();
                }

                else if (currentDamage == exhaustedAttackDamage)
                {
                    //call the play light attack sound if we are dealing the exhausted attack damage
                    meleeSound.PlayExhaustedAttackHitSound();
                }
            }
        }

        //we're gonna call this after ending a heavy attack!
        private void ResetCharge()
        {
            isCharged = false; isCharging = false;
        }

        private void StartCharge()
        {
            isCharging = true; Invoke(nameof(Charge), chargeUpTime);
        }

        private void Charge()
        {
            isCharged = true; EndCharge();
        }

        private void EndCharge()
        {
            isCharging = false;
        }

        private void Block(bool b)
        {
            isBlocking = b;

            //if we have the weapon sway reference, call the aim method on it as well (which we are using to block instead)
            if (weaponSway) weaponSway.Aim(b);
        }

        private void Animation()
        {
            //if we have the animation reference
            if (meleeAnimation)
            {
                //if we're charging our heavy attack, then play this anim
                if (isCharged || isCharging)
                {
                    meleeAnimation.PlayMeleeChargeAttack();
                    return;
                }

                //TODO - Add block_hit animation

                //if we're blocking and we're not attacking, play this anim
                else if (isBlocking && !isAttacking)
                {
                    meleeAnimation.PlayMeleeBlock();
                    return;
                }

                //if we're attacking
                else if (isAttacking)
                {
                    //if our damage is the light attack damage, play this anim
                    if (currentDamage == damage)
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

                //if we're not shooting, play this anim
                else if (!isAttacking)
                {
                    meleeAnimation.PlayMeleeIdle();
                    return;
                }
            }
        }

        public override ItemData GetItemData()
        {
            if (meleeData != null) return meleeData;

            else return null;
        }

        public override void SetItemData(ItemData itemData)
        {
            meleeData = (MeleeData)itemData;

            Init();
        }
    }
}