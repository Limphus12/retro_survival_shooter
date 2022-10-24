using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class Health : MonoBehaviour
    {
        public float healthPoints;

        public void TakeDamage(float amount)
        {
            healthPoints -= amount;

            if (healthPoints <= 0)
            {
                Kill();
            }
        }

        void Kill()
        {
            Destroy(gameObject);
        }
    }
}