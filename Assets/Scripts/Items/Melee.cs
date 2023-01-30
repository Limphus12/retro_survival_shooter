using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class Melee : Weapon
    {
        [Header("Attributes - Melee")]
        [SerializeField] private MeleeData meleeData;

        [Space]
        [SerializeField] private float attackRange;

        [Space]
        [SerializeField] private float lightAttackTimeToHit; //pair this with the apex of the swing animation

        [Space]
        public float heavyAttackRate;
        public float heavyAttackDamage;
        public float chargeUpTime;
        public float heavyAttackTimeToHit;

        [Space]
        public float exhaustedAttackRate;
        public float exhaustedAttackDamage;
        public float exhaustedAttackTimeToHit;

        //[Space]
        //[SerializeField] private MeleeSound meleeSound;

        [Space]
        [SerializeField] private WeaponSway weaponSway;
        //[SerializeField] private MeleeAnimation meleeAnimation;

        private bool isBlocking, isCharging, isCharged;

        private bool previousLeftMouseInput;

        //initialization
        protected override void Init()
        {
            if (!meleeData)
            {
                Debug.LogWarning("No Melee Data found for " + gameObject.name + "; Assign Melee Data!");
                return;
            }

            itemName = meleeData.itemName;
            itemWeight = meleeData.itemWeight;

            damage = meleeData.damage;
            attackRate = meleeData.attackRate;

            attackRange = meleeData.attackRange;
            lightAttackTimeToHit = meleeData.lightAttackTimeToHit;

            heavyAttackRate = meleeData.heavyAttackRate;
            heavyAttackDamage = meleeData.heavyAttackDamage;
            chargeUpTime = meleeData.chargeUpTime;
            heavyAttackTimeToHit = meleeData.heavyAttackTimeToHit;

            exhaustedAttackRate = meleeData.exhaustedAttackRate;
            exhaustedAttackDamage = meleeData.exhaustedAttackDamage;
            exhaustedAttackTimeToHit = meleeData.exhaustedAttackTimeToHit;
        }

        private void Update() => Inputs();

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
            //if were attacking already, dont do anything
            if (isAttacking) return;

            //if were not attacking and we press the l-mouse button
            if (leftMouseInput)
            {
                //check for stamina

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
                    //determine if we're blocking using the right mouse input
                    Block(rightMouseInput);
                }

                //if we were holding down the mouse in teh last frame
                else if (previousLeftMouseInput != leftMouseInput)
                {
                    //if we have charged
                    if (isCharged)
                    {
                        //do a heavy attack!
                        Debug.Log("Heavy Attack!");

                        ResetCharge();
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

                        Debug.Log("light attack!");

                        //do the light attack
                        StartAttack();
                    }
                }
            }
        }

        //starts attacking
        protected override void StartAttack()
        {
            isAttacking = true;

            //invoking attack after a delay to simulate the swinging of a melee weapon
            Invoke(nameof(Attack), lightAttackTimeToHit);

            //invoke end attack after our rate of fire
            Invoke(nameof(EndAttack), 1 / attackRate);
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
            //simple raycasting - prolly use this only for stabby knives in teh future?
            RaycastHit hit;
            if (Physics.Raycast(point.position, point.forward, out hit, attackRange))
            {
                IDamageable damageable = hit.transform.GetComponent<IDamageable>();

                if (damageable != null) damageable.Damage(damage);
            }

            //overlap box for melee combat - use in the future for larger swinging weapons
            //Collider[] colliders = Physics.OverlapBox(point.position, Vector3.one);

            //if (colliders.Length <= 0) return;

            //for (int i = 0; i < colliders.Length; i++)
            {
                //IDamageable damageable = colliders[i].transform.GetComponent<IDamageable>();

                //if (damageable != null) damageable.Damage(damage);
            }
        }

        //we're gonna call this after ending a heavy attack!
        private void ResetCharge()
        {
            isCharged = false;
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