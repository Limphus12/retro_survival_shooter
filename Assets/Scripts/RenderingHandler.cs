using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.limphus.retro_survival_shooter
{
    public class RenderingHandler : MonoBehaviour
    {
        private MeshRenderer[] meshRenderers;

        bool inRange = false, previousInRange = false;

        private Vector3 previousPosition;

        private void Awake() => Init();

        void Init()
        {
            meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();

            InvokeRepeating(nameof(CheckDistanceToPlayer), 0f, 1f);
        }

        void CheckDistanceToPlayer()
        {
            if (meshRenderers.Length == 0) return;

            Vector3 position = GameManager.Player.transform.position;

            if (position == previousPosition) return;

            float distance = Vector3.Distance(transform.position, GameManager.Player.transform.position);

            //using teh fog end distance!
            if (distance <= RenderSettings.fogEndDistance) inRange = true;
            else inRange = false;

            if (previousInRange != inRange) ToggleRenderers(inRange);

            previousInRange = inRange;
            previousPosition = position;
        }

        void ToggleRenderers(bool b)
        {
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                if (meshRenderer) meshRenderer.forceRenderingOff = !b;
            }
        }
    }
}