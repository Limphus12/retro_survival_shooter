using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class Hitbox : MonoBehaviour, IDamageable
    {
        public EntityStats Stats { get; set; }

        public void Damage(int amount)
        {
            if (Stats) Stats.Damage(amount);

            if (Stats.IsDead) Knockback();
        }

        private float knockbackForce = 70000f;

        private void Knockback()
        {
            float offsetF = 0.05f;
            Vector3 offset = new Vector3(UnityEngine.Random.Range(-offsetF, offsetF), UnityEngine.Random.Range(-offsetF, offsetF), UnityEngine.Random.Range(-offsetF, offsetF));

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb) rb.AddExplosionForce(knockbackForce, transform.position, 2f);
        }
    }
}