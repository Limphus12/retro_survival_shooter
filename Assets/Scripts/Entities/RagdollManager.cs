using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class RagdollManager : MonoBehaviour
    {
        private Rigidbody[] rigidbodies;

        private Animator animator;

        public EntityStats Stats { get; set; }

        private void Start()
        {
            rigidbodies = GetComponentsInChildren<Rigidbody>();
            Stats = GetComponentInParent<EntityStats>();

            SetHitboxes();

            ToggleRagdoll(false);
        }

        private void SetHitboxes()
        {
            foreach (Rigidbody rb in rigidbodies)
            {
                Hitbox hb = rb.gameObject.AddComponent<Hitbox>();
                hb.Stats = Stats;
            }
        }

        public void ToggleRagdoll(bool b)
        {
            foreach (Rigidbody rb in rigidbodies)
            {
                rb.isKinematic = !b;
            }

            if (animator) animator.enabled = !b;
        }
    }
}