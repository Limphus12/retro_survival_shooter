using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public enum MeleeState { IDLE, LIGHTATTACKING, HEAVYATTACKING, EXHAUSTEDATTACKING, BLOCKING, CHARGING }

    public class Melee : MonoBehaviour
    {
        [Header("Attributes - Melee")]
        [SerializeField] private MeleeData meleeData;

        private float lightAttackRate;
        private int lightAttackDamage;
        private float attackRange, lightAttackTimeToHit;
        private int lightAttackStaminaCost;

        private float heavyAttackRate;
        private int heavyAttackDamage;
        private float chargeUpTime, heavyAttackTimeToHit;
        private int heavyAttackStaminaCost;

        private float exhaustedAttackRate;
        private int exhaustedAttackDamage;
        private float exhaustedAttackTimeToHit;

        private PlayerStats playerStats;
        private Transform playerCamera;

        private MeleeSound meleeSound;
        private WeaponSway weaponSway;
        private MeleeAnimation meleeAnimation;

        protected bool isEquipped, isAttacking, isBlocking, isCharging, isCharged;

        private void Awake() => Init();

        public bool InUse()
        {
            if (isAttacking || isBlocking || isCharging) return true;

            else return false;
        }

        //initialization
        protected void Init()
        {
            InitStats(); InitEffects();

            if (!playerStats) playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
            if (!playerCamera) playerCamera = Camera.main.transform;
        }

        protected void InitStats()
        {
            //MELEE
            lightAttackDamage = meleeData.lightAttackDamage;
            lightAttackRate = meleeData.lightAttackRate;

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

            if (!playerStats) playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        }

        private void InitEffects()
        {
            if (!meleeSound) meleeSound = gameObject.GetComponent<MeleeSound>();

            if (!weaponSway) weaponSway = gameObject.GetComponent<WeaponSway>();

            if (!meleeAnimation) meleeAnimation = gameObject.GetComponent<MeleeAnimation>();
        }

        public void CheckInputs(bool meleeInput, bool previousMeleeInput, bool rightMouseInput)
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
                float stamina = playerStats.GetCurrentMeleeStamina();

                //if were not attacking and we press the l-mouse button or melee input
                if (meleeInput)
                {
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

                    //if we were holding down the mouse in teh last frame, and we have stamina
                    else if (previousMeleeInput != meleeInput && stamina > 0)
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
                            SetupAttack(lightAttackTimeToHit, lightAttackRate, lightAttackDamage, lightAttackStaminaCost);
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

        protected float currentTimeToHit, currentAttackRate;
        protected int currentDamage;

        //takes in a time to hit, attack rate & damage
        protected void SetupAttack(float hitTime, float attackRate, int damage, int staminaCost)
        {
            currentTimeToHit = hitTime;
            currentAttackRate = attackRate;
            currentDamage = damage;

            if (playerStats.GetCurrentMeleeStamina() <= staminaCost)
            {
                //reset the stamina replenishment tick & deplete all stamina
                playerStats.ResetMeleeStaminaReplenishTick(); playerStats.SetCurrentMeleeStamina(0);
            }

            else if (playerStats.GetCurrentMeleeStamina() > staminaCost)
            {
                //reset the stamina replenishment tick & deplete a certain amount of stamina
                playerStats.ResetMeleeStaminaReplenishTick(); playerStats.DepleteMeleeStamina(staminaCost);
            }

            //attack sounds - if we have the melee sound reference
            if (meleeSound)
            {
                if (currentDamage == lightAttackDamage)
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

            MeleeStartAttack();
        }

        //starts attacking
        protected void MeleeStartAttack()
        {
            isAttacking = true;

            //invoking attack after a delay to simulate the swinging of a melee weapon
            Invoke(nameof(MeleeAttack), currentTimeToHit);

            //invoke end attack after our rate of fire
            Invoke(nameof(MeleeEndAttack), 1 / currentAttackRate);
        }

        //shoots!
        protected void MeleeAttack()
        {
            //call the hit function, passing through the player camera
            MeleeHit(playerCamera);
        }

        //ends shooting
        protected void MeleeEndAttack()
        {
            isAttacking = false;
        }

        protected void MeleeHit(Transform point)
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
                if (currentDamage == lightAttackDamage)
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
        protected void ResetCharge()
        {
            isCharged = false; isCharging = false;
        }

        protected void StartCharge()
        {
            isCharging = true; Invoke(nameof(Charge), chargeUpTime);
        }

        protected void Charge()
        {
            isCharged = true; EndCharge();
        }

        protected void EndCharge()
        {
            isCharging = false;
        }

        private void InterruptCharge()
        {
            CancelInvoke(nameof(Charge));

            ResetCharge();
        }

        protected void Block(bool b)
        {
            isBlocking = b;

            //if we have the weapon sway reference, call the aim method on it as well (which we are using to block instead)
            if (weaponSway) weaponSway.Aim(b);
        }

        public MeleeState GetMeleeState()
        {
            if (isCharged || isCharging) return MeleeState.CHARGING;

            //TODO - Add block_hit animation

            else if (isBlocking && !isAttacking) return MeleeState.BLOCKING;

            else if (isAttacking)
            {
                //if our damage is the light attack damage, play this anim
                if (currentDamage == lightAttackDamage)
                {
                    return MeleeState.LIGHTATTACKING;
                }

                //if our damage is the heavy attack damage, play this anim
                else if (currentDamage == heavyAttackDamage)
                {
                    return MeleeState.HEAVYATTACKING;
                }

                //if our damage is the exhausted attack damage, play this anim
                else if (currentDamage == exhaustedAttackDamage)
                {
                    return MeleeState.EXHAUSTEDATTACKING;
                }

                else return MeleeState.IDLE;
            }

            else return MeleeState.IDLE;
        }

        public MeleeData GetMeleeData()
        {
            if (meleeData != null) return meleeData;

            else return null;
        }

        public void SetMeleeData(MeleeData meleeData)
        {
            this.meleeData = meleeData;

            Init();
        }

        public void Interrupt()
        {
            if (!InUse()) return;

            else
            {
                if (isBlocking) Block(false);
                if (isCharging) InterruptCharge();
            }
        }
    }
}