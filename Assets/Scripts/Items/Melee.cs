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
        [SerializeField] private float meleeRange;

        [Tooltip("Assign a value less than the rate of fire.")]
        [SerializeField] private float timeToHit; //I'd recommend half of the ROF because swinging should take the full ROF, whilst hitting should be at the apex of teh swing; halfway

        [Space]
        [SerializeField] private WeaponSway weaponSway;

        private bool isBlocking;

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
            rateOfFire = meleeData.rateOfFire;

            meleeRange = meleeData.meleeRange;
            timeToHit = meleeData.timeToHit;
        }

        private void Update() => Inputs();

        protected override void Inputs()
        {
            if (Input.GetMouseButtonDown(0)) leftMouseInput = true;
            else if (Input.GetMouseButtonUp(0)) leftMouseInput = false;

            if (Input.GetMouseButtonDown(1)) rightMouseInput = true;
            else if (Input.GetMouseButtonUp(1)) rightMouseInput = false;

            CheckShoot();
        }

        protected override void CheckShoot()
        {
            //if were swinging already, dont do anything
            if (isShooting) return;

            //if were not swinging and we press the l-mouse button, start swinging
            if (leftMouseInput) StartShoot();

            //else if were not shooting, determine if we're blocking
            else Block(rightMouseInput);
        }

        //starts shooting
        protected override void StartShoot()
        {
            isShooting = true;

            //invoking shoot after a delay to sim the swinging of a melee weapon
            Invoke(nameof(Shoot), 1 / timeToHit);

            //invoke end shoot after our rate of fire
            Invoke(nameof(EndShoot), 1 / rateOfFire);

            //if we have teh weapon sway reference, call the reload method (which we are using for the melee swinging)
            if (weaponSway) weaponSway.Reload(true);
        }

        //shoots!
        protected override void Shoot()
        {
            //call the hit function, passing through the player camera
            Hit(playerCamera);

            //if we have teh weapon sway reference, call the reload method (which we are using for the melee swinging)
            if (weaponSway) weaponSway.Reload(false);
        }

        //ends shooting
        protected override void EndShoot() => isShooting = false;

        protected override void Hit(Transform point)
        {
            //simple raycasting - prolly use this only for stabby knives in teh future?
            float range = 1;

            RaycastHit hit;
            if (Physics.Raycast(point.position, point.forward, out hit, range))
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