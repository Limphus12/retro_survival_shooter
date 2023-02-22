using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class TestDummy : MonoBehaviour, IDamageable
    {
        [SerializeField] private double health;

        public void Damage(float amount)
        {
            health -= amount;

            if (health <= 0)
            {
                Kill();
            }

        }

        private void Kill()
        {
            Destroy(gameObject);
        }
    }
}