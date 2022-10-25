using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class Melee : Weapon
    {
        [Tooltip("Assign a value less than the rate of fire.")] [SerializeField] private double timeToHit;

        private bool isBlocking;

        private void Update() => Inputs();

        protected override void Inputs()
        {
            leftMouseInput = Input.GetMouseButton(0);
            rightMouseInput = Input.GetMouseButton(1);

            CheckShoot();
        }

        protected override void CheckShoot()
        {
            //if were shooting already, dont do anything
            if (isShooting) return;

            //if were not shooting and we press the l-mouse button, start shooting
            if (leftMouseInput && !isBlocking) StartShoot();

            //else if were not shooting, determine if we're blocking
            else Block(rightMouseInput);
        }

        //starts shooting
        protected override void StartShoot()
        {
            isShooting = true;

            //invoking shoot after a delay to sim the swinging of a melee weapon
            Invoke(nameof(Shoot), 1 / (float)timeToHit);

            //invoke end shoot after our rate of fire
            Invoke(nameof(EndShoot), 1 / (float)rateOfFire);
        }

        //shoots!
        protected override void Shoot()
        {
            //call the hit function, passing through the player camera
            Hit(playerCamera);
        }

        //ends shooting
        protected override void EndShoot() => isShooting = false;

        protected override void Hit(Transform point)
        {
            //overlap box for melee combat
            Collider[] colliders = Physics.OverlapBox(point.position, Vector3.one);

            if (colliders.Length <= 0) return;

            for (int i = 0; i < colliders.Length; i++)
            {
                IDamageable damageable = colliders[i].transform.GetComponent<IDamageable>();

                if (damageable != null) damageable.Damage(damage);
            }
        }

        private void Block(bool b) => isBlocking = b;
    }
}