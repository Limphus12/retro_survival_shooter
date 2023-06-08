using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace com.limphus.retro_survival_shooter
{
    public class Pickup : MonoBehaviour
    {
        [SerializeField] protected int pickupAmount;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.gameObject == GameManager.Player) PickUp();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.transform.gameObject == GameManager.Player) PickUp();
        }

        protected virtual void PickUp() => Destroy();

        protected void Destroy() => Destroy(gameObject);
    }
}