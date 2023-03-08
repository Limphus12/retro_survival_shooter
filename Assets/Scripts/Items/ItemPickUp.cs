using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class ItemPickUp : MonoBehaviour
    {
        private Item itemScript;

        [SerializeField] private Vector3 offset, defaultPosition;

        private void Start() => itemScript = gameObject.GetComponent<Item>();
        
        private void OnTriggerEnter(Collider other)
        {
            NewPlayerInventory npi = other.GetComponent<NewPlayerInventory>();

            if (npi) npi.SetPickupItem(gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            NewPlayerInventory npi = other.GetComponent<NewPlayerInventory>();

            if (npi) npi.RemovePickupItem(gameObject);
        }

        public void PlaceOnFloor()
        {
            //do a raycast towards the floor
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
            {
                //and put us there! oh and reset our rotation
                transform.SetPositionAndRotation(hit.point + offset, Quaternion.Euler(0, 0, 0));
            }
        }

        public void PlaceInHand()
        {
            //and put us there! 
            transform.localPosition = defaultPosition;

            //oh and reset our rotation
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}