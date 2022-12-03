using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace com.limphus.retro_survival_shooter
{
    public class WorldBorder : MonoBehaviour
    {
        [SerializeField] private UnityEvent unityEvent;

        private bool check = false;

        private void OnTriggerEnter(Collider other)
        {
            //if the player enters this trigger
            if (other.CompareTag("Player") && !check)
            {
                //added this check to hopefully stop an inf loop.
                check = true;

                //call our event
                unityEvent.Invoke();
            }
        }
    }
}